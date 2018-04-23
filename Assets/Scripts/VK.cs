using Newtonsoft.Json;
using UnityEngine;
using System.Runtime.InteropServices;

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

	#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern void VKLogin();

	[DllImport("__Internal")]
	private static extern void VKInit(string appId, string gameObject);

	[DllImport("__Internal")]
	private static extern bool VKLoggedIn();

	[DllImport("__Internal")]
	private static extern bool WallPost(string link, string linkTitle, string text, string imageLink);
	#endif

    private void Start()
    {
        Init();
    }

	public void Init()
    {
		#if UNITY_ANDROID
        ajo = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        ajo.Call("VKinit", AppId, name);
		#elif UNITY_IOS
		VKInit("6227224", name);
		#endif
    }

    public static bool IsLoggedIn()
    {
		#if UNITY_ANDROID
        return ajo.Call<string>("VKcall", "isLoggedIn", "") == "1";
		#elif UNITY_IOS
		return VKLoggedIn();
		#endif
    }

    public static void Login(ActionResult callback)
    {        
		#if UNITY_ANDROID
        OnLogin += callback;
        ajo.Call<string>("VKcall", "login", JsonConvert.SerializeObject(scopes));
		#elif UNITY_IOS
		VKLogin();
		#endif
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
		#if UNITY_ANDROID
        ajo.Call("openVKShareDialog", message, link, linkTitle);
		#elif UNITY_IOS
		WallPost(link, linkTitle, message, "");
		#endif
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
