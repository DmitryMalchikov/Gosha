using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePicker : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject unityActivityClass;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
    static void _openFileSystem()
    {
        unityActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        unityActivityClass.Call("openGallery");
    }
#else
    static void _openFileSystem() { }
#endif

    public void OpenGallery()
    {
        _openFileSystem();
    }
}
