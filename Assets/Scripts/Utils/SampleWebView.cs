using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using RestSharp.Contrib;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class SampleWebView : MonoBehaviour
    {
        public static SampleWebView Instance { get; private set; }

        private static AndroidJavaObject unityActivityClass;
        private string _ua = "Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19";
#if !UNITY_ANDROID
    private WebViewObject webObj;
    private bool initialized = false;
#endif

        private void Awake()
        {
            Instance = this;
        }

        public void OpenWindow(string url)
        {
#if UNITY_ANDROID
            unityActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            unityActivityClass.Call("openBrowser", url, _ua, "Logins.html#");
#else
		if (!webObj)
		{
		webObj = GetComponent<WebViewObject>();		
		}
		webObj.LoadURL(url);
#endif
        }

        public void CallOnLoaded(string msg)
        {
            if (msg.Contains("Logins.html#"))
            {
                var vals = msg.Split('#')[1];

                var parsed = HttpUtility.ParseQueryString (vals);

                string token = parsed["access_token"];
                string refreshToken = parsed ["refresh_token"];
                string refreshExpire = parsed ["refresh_expires_in"];
                string email = parsed["userName"];
                LoginManager.UserToken = new AccessToken() { Token = token };
                LoginManager.Instance.CheckExternalRegister(refreshToken, refreshExpire, email);			
            }
            else
            {
                //status.text = msg;
            }
        }

        public void CallOnError(string msg)
        {
        }

        public void CloseWindow()
        {
#if UNITY_ANDROID
            AndroidJavaClass webViewActivity = new AndroidJavaClass("com.goshaplugins.WebViewActivity");
            webViewActivity.CallStatic("closeBrowser");
#else
		webObj.SetVisibility(false);
		webObj.DestroyWebView();
		initialized = false;
#endif
        }
    }
}
