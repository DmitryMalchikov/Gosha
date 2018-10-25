using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using uTasks;

public static class Extensions
{
    public static string GetPathByDataType(DataType type)
    {
        string fileName = string.Empty;
        string path = GameController.PersistentDataPath + "/Data";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        switch (type)
        {
            case DataType.Duels:
                fileName = "1.dat";
                break;
            case DataType.Shop:
                fileName = "2.dat";
                break;
            case DataType.Friends:
                fileName = "3.dat";
                break;
            case DataType.Suits:
                fileName = "4.dat";
                break;
            case DataType.Trades:
                fileName = "5.dat";
                break;
            case DataType.UserInfo:
                fileName = "6.dat";
                break;
        }

        return path + "/" + fileName;
    }

    public static void SaveJsonData(DataType type, string dataToSave)
    {
        string filePath = GetPathByDataType(type);  

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = File.Create(filePath);
        formatter.Serialize(fs, dataToSave);
        fs.Close();
    }

    public static void SaveJsonDataAsync(DataType type, string dataToSave)
    {
        Task.Run(() =>
        {
            SaveJsonData(type, dataToSave);
        });
    }

    public static string LoadJsonData(DataType type)
    {
        string filePath = GetPathByDataType(type);       

        if (!File.Exists(filePath))
        {
            return string.Empty;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = File.Open(filePath, FileMode.Open);
        string data = (string)formatter.Deserialize(fs);
        fs.Close();

        return data;
    }

    public static void RemoveJsonData(DataType type)
    {
        string filePath = GetPathByDataType(type);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public static string Sha1Hash(string input)
    {
        using (SHA1Managed sha1 = new SHA1Managed())
        {
            Encoding Encoding1252 = Encoding.GetEncoding(1252);
            var hash = sha1.ComputeHash(Encoding1252.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                // can be "x2" if you want lowercase
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }

    public static List<T> SubList<T>(this List<T> list, int startIndex, int amount)
    {
        List<T> res = new List<T>();

        T[] temp = new T[amount];
        list.CopyTo(startIndex, temp, 0, amount);

        res.AddRange(temp);

        return res;
    }

    public static void ShowGameObjects(List<GameObject> objects, bool show = true)
    {
        if (objects == null)
        {
            return;
        }
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(show);
        }
    }

    public static void Execute(this Action action)
    {
        if (action != null)
        {
            action();
        }
    }
}

public enum DataType
{
    Shop,
    Duels,
    Friends,
    Suits,
    Network,
    Trades,
    UserInfo
}
