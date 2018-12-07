using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Assets.Scripts.DTO;
using Assets.Scripts.Gameplay;
using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using Newtonsoft.Json;
using uTasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Network
{
    public static class NetworkHelper
    {
        public static bool HaveInternetConnection { get; private set; }

        public static UnityWebRequest CreateRequest(string url, string postParameters, string ContentType, List<Header> headers = null)
        {
            var data = Encoding.UTF8.GetBytes(postParameters);
            UnityWebRequest req = new UnityWebRequest(url, "POST");
            req.uploadHandler = new UploadHandlerRaw(data);
            req.downloadHandler = new DownloadHandlerBuffer();

            if (!string.IsNullOrEmpty(ContentType))
            {
                req.SetRequestHeader("Content-Type", ContentType);
                req.uploadHandler.contentType = ContentType;
            }

            if (headers != null)
            {
                for (int i = 0; i < headers.Count; i++)
                {
                    req.SetRequestHeader(headers[i].Name, headers[i].Value);
                }
            }
            req.SetRequestHeader("User-Agent", "GoshaGame");
            return req;
        }

        private static IEnumerator SendRequestBody(string url, object parameters, string contentType, Func<AnswerModel, IEnumerator> successMethod, Action<AnswerModel> errorMethod, string loadingPanelsKey, DataType type, bool forceUpdate, Action<AnswerModel> preSuccessMethod, Action finallyMethod)
        {
            AnswerModel response = new AnswerModel();
            yield return StartRequest(url, parameters, contentType, forceUpdate, type, response, loadingPanelsKey);

            try
            {
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    HandleRequestError(response, errorMethod);
                }
                else
                {
                    if (preSuccessMethod != null)
                    {
                        preSuccessMethod(response);
                    }
                    if (successMethod != null)
                    {
                        yield return successMethod(response);
                    }
                }
            }
            finally
            {
                if (finallyMethod != null)
                {
                    finallyMethod();
                }
                Extensions.ShowGameObjects(LoadingManager.GetPanelsByKey(loadingPanelsKey), false);
            }
        }

        public static IEnumerator SendRequest(string url, object parameters, string contentType, Action successMethod = null, Action<AnswerModel> errorMethod = null, string loadingPanelsKey = null, DataType type = DataType.Network, bool forceUpdate = false, Action<AnswerModel> preSuccessMethod = null, Action finallyMethod = null)
        {
            yield return SendRequestBody(url, parameters, contentType, answer => Empty(successMethod), errorMethod, loadingPanelsKey, type, forceUpdate, preSuccessMethod, finallyMethod);
        }

        public static IEnumerator SendRequest<T>(string url, object parameters, string contentType, Action<T> successMethod, Action<AnswerModel> errorMethod = null, string loadingPanelsKey = null, DataType type = DataType.Network, bool forceUpdate = false, Action<AnswerModel> preSuccessMethod = null, Action finallyMethod = null)
        {
            string pass = EncodeKeyHandler.GetKey(type);
            yield return SendRequestBody(url, parameters, contentType, answer => ParseDataAndCallSuccess(answer, successMethod, pass), errorMethod, loadingPanelsKey, type, forceUpdate, preSuccessMethod, finallyMethod);
        }

        private static IEnumerator Empty(Action action)
        {
            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static IEnumerator ParseDataAndCallSuccess<T>(AnswerModel response, Action<T> successMethod, string pass)
        {
            T data = default(T);

            var task = Task.Run(() => data = FileExtensions.TryParseData<T>(response.Text, pass));

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (data != null && successMethod != null)
            {
                successMethod(data);
            }
        }

        public static IEnumerator StartRequest(string url, object parameters, string contentType, bool forceUpdate, DataType type, AnswerModel response, string loadingPanelsKey)
        {
            HaveInternetConnection = true;
            forceUpdate = GetForceUpdate(type, forceUpdate);

            if (forceUpdate)
            {
                Extensions.ShowGameObjects(LoadingManager.GetPanelsByKey(loadingPanelsKey));
            }

            yield return WaitPersistentDataPath();

            InputString parms = new InputString();
            yield return SerializeParameters(parameters, parms);

            if (!forceUpdate)
            {
                LocalResponse(response, type);
            }

            if (response.StatusCode == 0)
            {
                forceUpdate = true;
                var req = CreateRequest(url, parms.Value, contentType, LoginManager.Instance.Headers);
                yield return SendRequest(req);
                SetResponse(response, req);
            }

            if (response.StatusCode == 0 && forceUpdate)
            {
                HaveInternetConnection = false;
                LocalResponse(response, type);
                forceUpdate = false;           
            }

            if (NeedSave(response.StatusCode, forceUpdate, type))
            {
                FileExtensions.SaveJsonDataAsync(type, response.Text);
            }
        }

        private static IEnumerator WaitPersistentDataPath()
        {
            while (GameController.PersistentDataPath == null)
            {
                yield return null;
            }
        }

        private static void LocalResponse(AnswerModel response, DataType type)
        {
            string data = FileExtensions.LoadJsonData(type);
            if (!string.IsNullOrEmpty(data) || type == DataType.UserInfo)
            {
                response.SetFields(new AnswerModel(data));
            }
        }

        private static IEnumerator SendRequest(UnityWebRequest req)
        {
#if !UNITY_5
            yield return req.SendWebRequest();
#else
        yield return req.Send();
#endif
        }

        private static void SetResponse(AnswerModel response, UnityWebRequest req)
        {
#if UNITY_5
                if (req.isError)
#else
            if (req.isHttpError || req.isNetworkError)
#endif
            {
                response.SetFields(HandleExceptionText(req.downloadHandler.text, (HttpStatusCode)req.responseCode));
            }
            else
            {
                response.SetFields(new AnswerModel(req.downloadHandler.text));
            }
        }

        private static bool NeedSave(HttpStatusCode code, bool wasForceUpdate, DataType type)
        {
            if (type == DataType.UserInfo || type == DataType.Network)
            {
                return false;
            }
            return code == HttpStatusCode.OK && wasForceUpdate;
        }

        private static bool GetForceUpdate(DataType type, bool inputForceUpdate)
        {
            //TODO: add if no connection always false
            if (type == DataType.Network)
            {
                return true;
            }
            if (type != DataType.Network && inputForceUpdate == false)
            {
                return HashManager.GetForceUpdate(type);
            }
            return inputForceUpdate;
        }

        private static IEnumerator<string> SerializeParameters(object parameters, InputString destination)
        {
            if (parameters == null)
            {
                yield return string.Empty;
            }

            var task = Task.Run(() =>
            {
                string parms = string.Empty;
                var s = parameters as string;
                if (s != null)
                {
                    parms = s;
                }
                else
                {
                    parms = JsonConvert.SerializeObject(parameters);
                }

                return parms;
            });

            while (!task.IsCompleted)
            {
                yield return null;
            }

            destination.Value = task.Result;
        }

        public static void HandleRequestError(AnswerModel response, Action<AnswerModel> errorMethod)
        {
            Debug.Log("Error");
            if (response.Errors != null)
            {
                Debug.Log(response.Errors);
            }
            if (errorMethod != null)
            {
                errorMethod(response);
            }
        }

        private static AnswerModel HandleExceptionText(string text, HttpStatusCode code)
        {
            Dictionary<string, IList<string>> errorCodes = null;
            try
            {
                var errors = JsonConvert.DeserializeObject<ErrorAnswer>(text);
                if (errors.ModelState != null && errors.ModelState.Any())
                {
                    errorCodes = errors.ModelState;
                }
                else
                {
                    errorCodes = new Dictionary<string, IList<string>> { { "Message", new List<string> { errors.Message } } };
                }
            }
            catch
            {
                errorCodes = new Dictionary<string, IList<string>> { { "Message", new List<string> { "FatalError" } } };
                //Canvaser.Errors.Enqueue(new WebException(text));
            }
            return new AnswerModel() { StatusCode = code, Errors = errorCodes };
        }

        public static IEnumerator SendImage(string fileName, string URL)
        {
            WWW localFile = new WWW("file://" + fileName);
            yield return localFile;
            if (localFile.error != null)
            {
                yield break;
            }

            if (localFile.texture == null)
            {
                yield break;
            }

            WWWForm postForm = new WWWForm();
            postForm.AddBinaryData("theFile", localFile.bytes, fileName, "image/png");

            var headers = postForm.headers;
            headers["Authorization"] = LoginManager.Instance.Headers.Find(h => h.Name == "Authorization").Value;
            byte[] rawData = postForm.data;

            WWW upload = new WWW(URL, rawData, headers);
            yield return upload;
            if (upload.error == null)
                Debug.Log("upload done :");
            else
                Debug.Log("Error during upload: " + upload.error + " URL: " + URL);

            LoginManager.Instance.GetUserImage();
        }
    }
}
