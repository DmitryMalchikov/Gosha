using System;
using UnityEngine;

public class CoroutineManager : Singleton<CoroutineManager>
{
    private static WaitForEndOfFrame _frame;
    public static WaitForEndOfFrame Frame
    {
        get
        {
            if (_frame == null)
            {
                _frame = new WaitForEndOfFrame();
            }

            return _frame;
        }
    }

    public static Coroutine SendRequest(string url, object parameters, string contentType, Action successMethod = null, Action<AnswerModel> errorMethod = null, string loadingPanelsKey = null, DataType type = DataType.Network, bool forceUpdate = false, Action<AnswerModel> preSuccessMethod = null, Action finallyMethod = null)
    {
        return Instance.StartCoroutine(NetworkHelper.SendRequest(url, parameters, contentType, successMethod, errorMethod, loadingPanelsKey, type, forceUpdate, preSuccessMethod, finallyMethod));
    }

    public static Coroutine SendRequest(string url, object parameters,Action successMethod = null, Action<AnswerModel> errorMethod = null, string loadingPanelsKey = null, DataType type = DataType.Network, bool forceUpdate = false, Action<AnswerModel> preSuccessMethod = null, Action finallyMethod = null)
    {
        String contentType = "application/json";
        return Instance.StartCoroutine(NetworkHelper.SendRequest(url, parameters, contentType, successMethod, errorMethod, loadingPanelsKey, type, forceUpdate, preSuccessMethod, finallyMethod));
    }

    public static Coroutine SendRequest<T>(string url, object parameters, string contentType, Action<T> successMethod, Action<AnswerModel> errorMethod = null, string loadingPanelsKey = null, DataType type = DataType.Network, bool forceUpdate = false, Action < AnswerModel> preSuccessMethod = null, Action finallyMethod = null)
    {
        return Instance.StartCoroutine(NetworkHelper.SendRequest<T>(url, parameters, contentType, successMethod, errorMethod, loadingPanelsKey, type, forceUpdate, preSuccessMethod, finallyMethod));
    }

    public static Coroutine SendRequest<T>(string url, object parameters, Action<T> successMethod, Action<AnswerModel> errorMethod = null, string loadingPanelsKey = null, DataType type = DataType.Network, bool forceUpdate = false, Action<AnswerModel> preSuccessMethod = null, Action finallyMethod = null)
    {
        String contentType = "application/json";
        return Instance.StartCoroutine(NetworkHelper.SendRequest<T>(url, parameters, contentType, successMethod, errorMethod, loadingPanelsKey, type, forceUpdate, preSuccessMethod, finallyMethod));
    }
}
