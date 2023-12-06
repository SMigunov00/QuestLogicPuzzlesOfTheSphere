using TMPro;
using UnityEngine;

namespace Runtime.Localization
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string _key;

        private Localizator _localizator;
        private TextMeshProUGUI _text;

        private void Awake()
        {
            if (_localizator == null)
                _localizator = FindObjectOfType<Localizator>().GetComponent<Localizator>();

            if (_text == null)
                _text = GetComponent<TextMeshProUGUI>();

            _localizator.OnLanguageChanged += UpdateText;
        }

        private void Start()
        {
            UpdateText();
        }

        private void OnDestroy()
        {
            _localizator.OnLanguageChanged -= UpdateText;
        }

        virtual protected void UpdateText()
        {
            if (gameObject == null) return;

            if (_localizator == null)
                _localizator = FindObjectOfType<Localizator>().GetComponent<Localizator>();

            if (_text == null)
                _text = GetComponent<TextMeshProUGUI>();

            _text.text = _localizator.GetLocalizedValue(_key);
        }
    }
}