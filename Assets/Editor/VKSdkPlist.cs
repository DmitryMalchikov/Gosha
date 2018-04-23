using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using PlistCS;

public class VKSdkPlist : MonoBehaviour {
	
	[PostProcessBuild]
	public static void Process(BuildTarget target, string path)
	{
		Debug.Log("iOS URL scheme post process");
		#if UNITY_5
		if (target != BuildTarget.iOS)
		#else
		if (target != BuildTarget.iPhone)
		#endif
		{
			Debug.Log("Bad target: " + target);
			return;
		}

		UpdatePlist(path);
	}

	private static void UpdatePlist(string path)
	{
		const string fileName = "Info.plist";
		string fullPath = Path.Combine(path, fileName);

		Dictionary<string, object> dict;
		dict = (Dictionary<string, object>)Plist.readPlist(fullPath);

		dict["CFBundleURLTypes"] = new List<object> {
			new Dictionary<string,object> {
				{ "CFBundleURLName", "vk-sdk" },
				{ "CFBundleURLSchemes", new List<object> { "vk6227224" } }
			}
		};

		Plist.writeXml(dict, fullPath);
	}
}
