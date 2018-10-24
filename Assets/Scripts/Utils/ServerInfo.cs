using System.Text;
using UnityEngine;

public class ServerInfo : MonoBehaviour
{
    public string IP = "10.10.0.91";
    static string urlPart = "";

    private void Awake()
    {
        StringBuilder res = new StringBuilder(IP);
        urlPart = res.ToString();
    }

    public static void SetUrl(ref string path)
    {
        path = urlPart + path;
    }
}
