using UnityEngine;

public class ButtonSwitchLanguage : MonoBehaviour
{
    [SerializeField] private Localisator _localisator;

    public void OnButtonClick() => _localisator.CurrentLanguage = name;
}