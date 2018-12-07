using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.Gameplay;
using Assets.Scripts.Managers;
using Encryptor;
using Newtonsoft.Json;
using uTasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    class FileExtensions
    {
        private static KeyEncryptor _encryptor;
        private static string _deviceId;
        private static BinaryFormatter _binaryFormatter;

        static FileExtensions()
        {
            _encryptor = new KeyEncryptor("saltsalt");
            _deviceId = GameController.DeviceId;
            _binaryFormatter = new BinaryFormatter();
        }

        public static FilePath GetPathByDataType(DataType type)
        {
            FilePath fullPath = new FilePath(LoginManager.LocalUser, type);
            return fullPath;
        }

        public static void SaveJsonData(DataType type, string dataToSave)
        {
            var filePath = GetPathByDataType(type);

            using (FileStream fs = File.Create(filePath.FullFilePath))
            {
                string encodedText = _encryptor.Encrypt(dataToSave, _deviceId);
                _binaryFormatter.Serialize(fs, encodedText);
            }
        }

        public static void SaveJsonDataAsync(DataType type, string dataToSave)
        {
            Task.Run(() =>
            {
                SaveJsonData(type, dataToSave);
            });
        }

        public static void SaveJsonDataAsync(DataType type, object dataToSave)
        {
            Task.Run(() =>
            {
                string data = JsonConvert.SerializeObject(dataToSave);
                SaveJsonData(type, data);
            });
        }

        public static string LoadJsonData(DataType type)
        {
            var filePath = GetPathByDataType(type);

            if (filePath.Location == FileLocation.StreamingAssets)
            {
                string res = LoadTextFromStreamingAssets(filePath.FullFilePath).GetEnumeratorResult();
                return res;
            }

            if (!File.Exists(filePath.FullFilePath))
            {
                return string.Empty;
            }

            using (FileStream fs = File.Open(filePath.FullFilePath, System.IO.FileMode.Open))
            {
                string data = (string)_binaryFormatter.Deserialize(fs);
                return data;
            }
        }

        public static void RemoveJsonData(DataType type)
        {
            var filePath = GetPathByDataType(type);

            if (File.Exists(filePath.FullFilePath))
            {
                File.Delete(filePath.FullFilePath);
            }
        }

        public static void RemoveAllCachedData()
        {
            string folderPath = Path.Combine(GameController.PersistentDataPath, "Data");
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }

        public static T TryParseData<T>(string input, string key = null)
        {
            T result = default(T);

            if (string.IsNullOrEmpty(key))
            {
                key = _deviceId;
            }

            try
            {
                result = JsonConvert.DeserializeObject<T>(input);
            }
            catch
            {
                string decodedString = _encryptor.Decrypt(input, key);
                result = JsonConvert.DeserializeObject<T>(decodedString);
            }

            return result;
        }

        public static IEnumerator<string> LoadTextFromStreamingAssets(string filePath, Action<string> onLoadEnd = null)
        {
            string dataAsJson = string.Empty;
#if UNITY_ANDROID && !UNITY_EDITOR
        var www = new WWW(filePath);
        while (!www.isDone)
        {
            yield return null;
        }

        if (string.IsNullOrEmpty(www.error))
        {
            dataAsJson = www.text;
        }
#else
            if (File.Exists(filePath))
            {
                dataAsJson = File.ReadAllText(filePath);
            }
#endif
            if (onLoadEnd != null)
            {
                onLoadEnd(dataAsJson);
            }
            else
            {
                yield return dataAsJson;
            }
        }
    }
}
