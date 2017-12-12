using System;
using System.Text;
using UnityEngine;

public class ServerInfo : MonoBehaviour
{

    public string IP = "10.10.0.91";
    public string Port = "58629";

    static string urlPart = "";

    private void Awake()
    {
        StringBuilder res = new StringBuilder(IP);
        if (!string.IsNullOrEmpty(Port))
        {
            res.Append(':' + Port);
        }

        urlPart = res.ToString();
    }

    public static string GetUrl(string path)
    {
        return urlPart + path;
    }
}
