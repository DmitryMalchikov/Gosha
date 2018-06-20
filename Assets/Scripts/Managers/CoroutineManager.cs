using System;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
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
