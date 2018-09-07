using UnityEngine;
using System.Runtime.InteropServices;
using RestSharp.Contrib;

public class GoshaAuthManager : MonoBehaviour
{
	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void GoshaInit(string gameObject);

	public void OnGoshaAuthorization(string url){

		url = url.Split ('#')[1];
		var parsed = HttpUtility.ParseQueryString (url);

		string token = parsed["access_token"];
		string refreshToken = parsed ["refresh_token"];
		string refreshExpire = parsed ["refresh_expires_in"];
		string email = parsed["userName"];
		LoginManager.userToken = new AccessToken() { Token = token };
		LoginManager.Instance.CheckExternalRegister(refreshToken, refreshExpire, email);
	}

	void Start(){
		GoshaInit (name);
	}
#endif
}
