using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Runtime.Localization
{
    public class Localizator : MonoBehaviour
    {
        public static bool localizedTextIsReady = false;
        private string _currentLanguage;
        private Dictionary<string, string> _localizedText;

        public delegate void ChangeLanguageText();
        public event ChangeLanguageText OnLanguageChanged;

        private void Awake()
        {
            if (!PlayerPrefs.HasKey("Language"))
            {
                if (Application.systemLanguage == SystemLanguage.Russian
                    || Application.systemLanguage == SystemLanguage.Ukrainian
                    || Application.systemLanguage == SystemLanguage.Belarusian)
                    PlayerPrefs.SetString("Language", "ru_RU");
                else
                    PlayerPrefs.SetString("Language", "en_US");
            }
            _currentLanguage = PlayerPrefs.GetString("Language");
            StartCoroutine(GetText(_currentLanguage));
        }

        private IEnumerator GetText(string languageName)
        {
            string path = Application.streamingAssetsPath + "/Languages/" + languageName + ".json";
            string dataAsJson;
            if (Application.platform == RuntimePlatform.Android)
            {
                UnityWebRequest unityWebRequest = new UnityWebRequest(path);
                unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                yield return unityWebRequest.SendWebRequest();

                dataAsJson = unityWebRequest.downloadHandler.text;
            }
            else
            {
                dataAsJson = File.ReadAllText(path);
            }

            LocalizationData localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            _localizedText = new Dictionary<string, string>();
            for (int i = 0; i < localizationData.Items.Length; i++)
                _localizedText.Add(localizationData.Items[i].Key, localizationData.Items[i].Value);

            PlayerPrefs.SetString("Language", languageName);
            _currentLanguage = PlayerPrefs.GetString("Language");
            localizedTextIsReady = true;

            OnLanguageChanged?.Invoke();
        }

        public string GetLocalizedValue(string key)
        {
            if (_localizedText.ContainsKey(key))
                return _localizedText[key];
            else
                throw new Exception("Localized text with key \"" + key + "\" not found");
        }

        public string CurrentLanguage
        {
            get
            {
                return _currentLanguage;
            }
            set
            {
                StartCoroutine(GetText(value));
            }
        }

        public bool IsReady
        {
            get
            {
                return localizedTextIsReady;
            }
        }
    }
}