using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    private static Dictionary<string, string> localizedText;

    public string NotFoundKey = "Missing text";
    public static Language CurrentLanguage = Language.EN;
    public static bool IsReady = false;

    private static Dictionary<Language, string> localizationFileNames = new Dictionary<Language, string>()
    {
        { Language.EN, "localization_eng.json" },
        { Language.RU, "localization_ru.json" }
    };

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    IEnumerator Start()
    {
        var lang = PlayerPrefs.GetInt("language");
        if (lang != 0)
        {
            CurrentLanguage = (Language)lang;
        }

        yield return LoadLocalizedText(localizationFileNames[CurrentLanguage]);
        Canvaser.Instance.LanguageDropdown.value = (int)CurrentLanguage - 1;
    }


    public IEnumerator LoadLocalizedText(string fileName)
    {

        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
#if UNITY_ANDROID && !UNITY_EDITOR
        var www = new WWW(filePath);
        yield return www;
        
        if (string.IsNullOrEmpty(www.error))
        {
            string dataAsJson = www.text;
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
        }
        else
        {
            Debug.Log(www.error);
        }
#else
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
        }
        yield return null;
#endif
        IsReady = true;
        
    }

    public void ChangeLanguage(int lang)
    {
        IsReady = false;
        CurrentLanguage = (Language)lang + 1;

        StartCoroutine(ChangeLanguage());
    }  

    IEnumerator ChangeLanguage()
    {
        yield return LoadLocalizedText(localizationFileNames[CurrentLanguage]);

        var texts = FindObjectsOfType<LocalizedText>();
        PlayerPrefs.SetInt("language", (int)CurrentLanguage);

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].SetText();
            yield return null;
        }
    }

    public static string GetLocalizedValue(string key)
    {
        if (localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }

        return Instance.NotFoundKey;
    }
}

public enum Language
{
    RU = 1, EN
}