using UnityEngine;

namespace Runtime.Localization
{
    public class ButtonSwitchLanguage : MonoBehaviour
    {
        [SerializeField] private Localizator _localizator;

        public void OnButtonClick() => _localizator.CurrentLanguage = name;
    }
}