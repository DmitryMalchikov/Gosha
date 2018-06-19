using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Utils
{
    private static string shluha = "supershluha2";

    public static string IntervalToString(this TimeSpan time)
    {
        StringBuilder res = new StringBuilder();

        if (time.Days > 0)
        {
            res.Append(time.Days + LocalizationManager.GetLocalizedValue("day"));
        }
        if (time.Hours > 0)
        {
            res.Append(time.Hours + LocalizationManager.GetLocalizedValue("hour"));
        }

        res.Append(time.Minutes + LocalizationManager.GetLocalizedValue("minute"));

        return res.ToString();
    }

    public static string CalculateMD5Hash(string input)

    {
        // step 1, calculate MD5 hash from input

        MD5 md5 = MD5.Create();

        byte[] inputBytes = Encoding.ASCII.GetBytes(input + shluha);

        byte[] hash = md5.ComputeHash(inputBytes);

        // step 2, convert byte array to hex string

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }

        return sb.ToString();

    }
}

public static class CoroutineUtil
{
    public static IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }
}
