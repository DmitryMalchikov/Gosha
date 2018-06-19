using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Extensions
{

    public static List<GameObject> LoadingPanels(this MonoBehaviour element)
    {
        return element.GetComponent<Panel>().LoadingPanels;
    }

    public static List<GameObject> LoadingPanels(this MonoBehaviour element, int index)
    {
        return new List<GameObject> { element.GetComponent<Panel>().LoadingPanels[index] };
    }

    public static List<GameObject> LoadingPanels(this GameObject element)
    {
        return element.GetComponent<Panel>().LoadingPanels;
    }

    public static void SaveJsonData(DataType type, string dataToSave)
    {
        string fileName = string.Empty;
        string path = Path.Combine(GameController.PersistentDataPath, "Data");
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
        }

        BinaryFormatter formatter = new BinaryFormatter();
        path = Path.Combine(path, fileName);
        FileStream fs = File.Create(path);
        formatter.Serialize(fs, dataToSave);
        fs.Close();

        //File.wri(path, dataToSave);
    }

    public static string LoadJsonData(DataType type)
    {
        string fileName = string.Empty;
        string path = Path.Combine(GameController.PersistentDataPath, "Data");

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
        }

        path = Path.Combine(path, fileName);

        if (!File.Exists(path))
        {
            return string.Empty;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = File.Open(path, FileMode.Open);
        string data = (string)formatter.Deserialize(fs);
        fs.Close();

        return data;
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
}

public enum DataType
{
    Shop, Duels, Friends, Suits, Network
}
