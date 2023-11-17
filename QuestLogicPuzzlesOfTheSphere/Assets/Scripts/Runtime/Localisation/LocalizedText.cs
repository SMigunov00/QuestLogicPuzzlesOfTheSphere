using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _key;

    private Localisator _localisator;
    private TextMeshProUGUI _text;

    void Awake()
    {
        if (_localisator == null)
            _localisator = FindObjectOfType<Localisator>().GetComponent<Localisator>();

        if (_text == null)
            _text = GetComponent<TextMeshProUGUI>();

        _localisator.OnLanguageChanged += UpdateText;
    }

    void Start()
    {
        UpdateText();
    }

    private void OnDestroy()
    {
        _localisator.OnLanguageChanged -= UpdateText;
    }

    virtual protected void UpdateText()
    {
        if (gameObject == null) return;

        if (_localisator == null)
            _localisator = FindObjectOfType<Localisator>().GetComponent<Localisator>();

        if (_text == null)
            _text = GetComponent<TextMeshProUGUI>();

        _text.text = _localisator.GetLocalizedValue(_key);
    }
}