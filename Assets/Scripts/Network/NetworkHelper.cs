using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using uTasks;

public static class NetworkHelper
{
    public static UnityWebRequest CreateRequest(string url, string postParameters, string ContentType, List<Header> headers = null)
    {
        if (string.IsNullOrEmpty(postParameters))
        {
            postParameters = "sas";
        }
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
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
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
            else
            {
                HandleRequestError(response, errorMethod);
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
        yield return SendRequestBody(url, parameters, contentType, answer => ParseDataAndCallSuccess(answer, successMethod), errorMethod, loadingPanelsKey, type, forceUpdate, preSuccessMethod, finallyMethod);
    }

    private static IEnumerator Empty(Action action)
    {
        if (action != null)
        {
            action();
        }
        yield break;
    }

    private static IEnumerator ParseDataAndCallSuccess<T>(AnswerModel response, Action<T> successMethod)
    {
        T data = default(T);

        var task = Task.Run(() => data = JsonConvert.DeserializeObject<T>(response.Text));

        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (successMethod != null)
        {
            successMethod(data);
        }
    }

    public static IEnumerator StartRequest(string url, object parameters, string contentType, bool forceUpdate, DataType type, AnswerModel response, string loadingPanelsKey)
    {
        forceUpdate = GetForceUpdate(type, forceUpdate);

        if (forceUpdate)
        {
            Extensions.ShowGameObjects(LoadingManager.GetPanelsByKey(loadingPanelsKey));
        }

        while (GameController.PersistentDataPath == null)
        {
            yield return null;
        }
        string parms = string.Empty;
        
        IEnumerator e = SerializeParameters(parameters);
        yield return e;
        parms = e.Current as string;

        if (!forceUpdate)
        {
            string data = Extensions.LoadJsonData(type);
            if (!string.IsNullOrEmpty(data) || type == DataType.UserInfo)
            {
                response.SetFields(new AnswerModel(data));
            }
        }
        if (response.StatusCode == 0)
        {
            forceUpdate = true;
            LoadingManager.PanelKeyToEnable = loadingPanelsKey;
            var req = CreateRequest(url, parms, contentType, LoginManager.Instance.Headers);
            yield return SendRequest(req);
            SetResponse(response, req);
        }

        if (NeedSave(response.StatusCode, forceUpdate, type))
        {
            Extensions.SaveJsonDataAsync(type, response.Text);
        }
    }

    private static IEnumerator SendRequest(UnityWebRequest req)
    {
#if UNITY_2017
        yield return req.SendWebRequest();
#elif UNITY_5
             yield return req.Send();
#endif
    }

    private static void SetResponse(AnswerModel response, UnityWebRequest req)
    {
#if UNITY_2017
        if (req.isHttpError)
#elif UNITY_5
                if (req.isError)
#endif
        {
            response.SetFields(HandleExceptionText(req.error, (HttpStatusCode)req.responseCode));
        }
#if UNITY_2017
        else if (req.isNetworkError)
        {
            response.SetFields(HandleExceptionText(req.error, (HttpStatusCode)req.responseCode));
        }
#endif
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
        if (code == HttpStatusCode.OK && wasForceUpdate)
        {
            return true;
        }
        return false;
    }

    private static bool GetForceUpdate(DataType type, bool inputForceUpdate)
    {
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

    private static IEnumerator SerializeParameters(object parameters)
    {
        if (parameters == null)
        {
            yield return string.Empty;
        }

        var task = Task.Run(() =>
        {
            string parms = string.Empty;

            if (parameters is string)
            {
                parms = parameters as string;                
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
        yield return task.Result;
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
        Debug.Log(text);
        var errors = new ErrorAnswer();
        Dictionary<string, IList<string>> errorCodes = null;
        try
        {
            errors = JsonConvert.DeserializeObject<ErrorAnswer>(text);
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
            Canvaser.Errors.Enqueue(new WebException(text));
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
