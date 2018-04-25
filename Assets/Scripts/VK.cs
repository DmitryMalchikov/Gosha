using Newtonsoft.Json;
using UnityEngine;
using System.Runtime.InteropServices;

public class VK : MonoBehaviour
{
    //public delegate void CallBack(object obj);
    public delegate void ActionResult(string msg);

    static event ActionResult OnLogin;
    static event ActionResult OnShare;
    static public string AppId = "6227224";

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
	private static extern void VKLogout();

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
        OnLogin += callback;
		#if UNITY_ANDROID
        ajo.Call<string>("VKcall", "login", JsonConvert.SerializeObject(scopes));
#elif UNITY_IOS
		VKLogin();
#endif
    }

    public void OnLoginError()
    {
		if (OnLogin != null) {
			OnLogin ("error");
			OnLogin = null;
		}
    }

    public void OnLoginSuccess()
    {
		if (OnLogin != null) {
			OnLogin ("success");
			OnLogin = null;
		}
    }

    public void OnShareSuccess()
    {
		if (OnShare != null) {
			OnShare ("");
			OnShare = null;
		}
    }

    public void OnShareError(string message)
    {
		if (OnShare != null) {
			OnShare (message);
			OnShare = null;
		}
    }

    public static void WallPost(string message, string link, string linkTitle, string imageUrl, ActionResult callback)
    {
        OnShare += callback;
#if UNITY_ANDROID
        ajo.Call("openVKShareDialog", message, link, linkTitle, imageUrl);
#elif UNITY_IOS
		WallPost(link, linkTitle, message, imageUrl);
#endif
    }

    public static void LogOut()
    {
#if UNITY_IOS
        VKLogout ();
#elif UNITY_ANDROID
#endif
    }
}
