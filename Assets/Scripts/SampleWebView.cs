using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class SampleWebView : MonoBehaviour
{
    public static SampleWebView Instance { get; private set; }

    private static AndroidJavaObject unityActivityClass;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenWindow(string url)
    {
        unityActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        unityActivityClass.Call("openBrowser", url, "Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19", "Logins.html#");
    }

    public void CallOnLoaded(string msg)
    {
        if (msg.Contains("Logins.html#"))
        {
            var vals = msg.Split('*');
            if (vals.Length < 2) return;

            string url = vals[0];
            string cookies = vals[1];
            Uri uri = new Uri(url);

            //status.text = url;
            string[] tokenParams = url.Split('#')[1].Split('&');
            string token = tokenParams[0].Split('=')[1];
            string expireDate = tokenParams[2];
            var s = DateTime.Now.AddSeconds(double.Parse(expireDate.Split('=')[1]));
            //CloseWindow();
            LoginManager.userToken = new AccessToken() { Token = token, ExpireDate = s };
            LoginManager.Instance.CheckExternalRegister();
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
        AndroidJavaClass webViewActivity = new AndroidJavaClass("com.goshaplugins.WebViewActivity");
        webViewActivity.CallStatic("closeBrowser");
    }
}
