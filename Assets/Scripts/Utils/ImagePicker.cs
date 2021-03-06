﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

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
	#elif UNITY_IOS
	[DllImport("__Internal")]
	private static extern void Unimgpicker_show(string title, string outputFileName, int maxSize);

	static void _openFileSystem()
	{
		Unimgpicker_show ("avatar", "avatar", 10);
	}
#else 
    static void _openFileSystem() { }
#endif

    public void OpenGallery()
    {
        _openFileSystem();
    }
}
