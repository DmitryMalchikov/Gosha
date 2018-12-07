using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        public string NotFoundKey = "Missing text";

        private static Dictionary<string, string> _localizedText;
        private static Dictionary<Language, string> _localizationFileNames = new Dictionary<Language, string>()
        {
            { Language.EN, "localization_eng.json" },
            { Language.RU, "localization_ru.json" }
        };

        public static Language CurrentLanguage { get; private set; }
        public static bool IsReady { get; private set; }

        public static string GetValue(string ru, string eng)
        {
            if (CurrentLanguage == Language.RU)
            {
                return ru;
            }

            return eng;
        }
        
        IEnumerator Start()
        {
            var lang = PlayerPrefs.GetInt("language");
            if (lang != 0)
            {
                CurrentLanguage = (Language)lang;
            }
            else
            {
                if (Application.systemLanguage == SystemLanguage.Russian)
                {
                    CurrentLanguage = Language.RU;
                }
                else
                {
                    CurrentLanguage = Language.EN;
                }
            }

            yield return LoadLocalizedText(_localizationFileNames[CurrentLanguage]);
        }


        public IEnumerator LoadLocalizedText(string fileName)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            yield return StartCoroutine(FileExtensions.LoadTextFromStreamingAssets(filePath, ParseJsonIntoDictionary));
        }

        public void ParseJsonIntoDictionary(string dataAsJson)
        {
            _localizedText = new Dictionary<string, string>();
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                _localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            IsReady = true;
        }

        public void ChangeLanguage(int lang)
        {
            IsReady = false;
            CurrentLanguage = (Language)lang + 1;

            StartCoroutine(ChangeLanguage());
        }

        private IEnumerator ChangeLanguage()
        {
            yield return LoadLocalizedText(_localizationFileNames[CurrentLanguage]);

            var texts = FindObjectsOfType<LocalizedText>();
            PlayerPrefs.SetInt("language", (int)CurrentLanguage);

            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].SetText();
            }

            if (LoginManager.User != null)
            {
                Canvaser.Instance.SBonuses.SetStartBonuses(LoginManager.User.Bonuses);
            }
        }

        public static string GetLocalizedValue(string key)
        {
            if (_localizedText.ContainsKey(key))
            {
                return _localizedText[key].Replace("\\n", Environment.NewLine);
            }

            return Instance.NotFoundKey;
        }
    }

    public enum Language
    {
        RU = 1, EN
    }
}