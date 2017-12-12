using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VK : MonoBehaviour {
    //public delegate void CallBack(object obj);
    public delegate void ActionResult(string msg);

    static event ActionResult OnLogin;
    static event ActionResult OnShare;
    static public string AppId= "6227224";

    //static private Dictionary<string, CallBack> mapFunction = new Dictionary<string, CallBack>();
    static AndroidJavaObject ajo;
    static string[] scopes = { "wall", "offline" };

    //static string _apiCall(string method, string param)
    //{
    //    return ajo.Call<string>("VKMethodCall", method, param);
    //}

    //static public void Api(string method, Dictionary<string, object> data, CallBack onResponse)
    //{
    //    mapFunction[_apiCall(method, JsonConvert.SerializeObject(data))] = onResponse;
    //}

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        ajo = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        ajo.Call("VKinit", AppId, name);
    }

    public static bool IsLoggedIn()
    {
        return ajo.Call<string>("VKcall", "isLoggedIn", "") == "1";
    }

    public static void Login(ActionResult callback)
    {        
        OnLogin += callback;
        ajo.Call<string>("VKcall", "login", JsonConvert.SerializeObject(scopes));
    }

    public void OnLoginError()
    {
        OnLogin("error");
        OnLogin = null;
    }

    public void OnLoginSuccess()
    {
        OnLogin("success");
        OnLogin = null;
    }

    public void OnShareSuccess()
    {
        OnShare("");
        OnShare = null;
    }

    public void OnShareError(string message)
    {
        OnShare(message);
        OnShare = null;
    }

    public static void WallPost(string message, string link, string linkTitle, ActionResult callback)
    {
        OnShare += callback;
        ajo.Call("openVKShareDialog", message, link, linkTitle);
        //Dictionary<string, object> obj1 = new Dictionary<string, object>();
        //obj1.Add("message", message);
        //Api("wall.post", obj1, callback);
    }

    //public void call(string data)
    //{
    //    Dictionary<string, object> request = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

    //    string requestName = request.Keys.ToList()[0];
    //    if (requestName.IndexOf("response") == 0)
    //    {
    //        int indexStartId = 8;
    //        if (requestName.IndexOf("responseError") == 0)
    //        {
    //            indexStartId = 13;
    //        }
    //        string idRequare = requestName.Substring(indexStartId);
    //        CallBack myFunc = mapFunction[idRequare];
    //        mapFunction.Remove(idRequare);

    //        object requestData = request[requestName];
    //        myFunc(requestData);
    //    }
    //}
}
