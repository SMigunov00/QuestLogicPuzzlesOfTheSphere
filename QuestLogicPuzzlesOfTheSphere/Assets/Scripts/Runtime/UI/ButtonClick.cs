using Runtime.Save;
using Runtime.SceneObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Runtime.UI
{
    public class ButtonClick : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent EventButtonClick;
        [SerializeField] private string _nameAnimation;
        private float _speedButtonClick;
        private Animator _anim;

        private GameSettingsData _settingsData = new GameSettingsData();

        private void OnEnable()
        {
            Tweaker.OnChangeSpeedButtonClick += HandleChangeSpeedButtonClick;
            _anim = GetComponent<Animator>();

            if (PlayerPrefs.HasKey("SavedGameSettingsData"))
                _settingsData = JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString("SavedGameSettingsData"));

            _speedButtonClick = _settingsData.SpeedButtonCLick;

        }

        private void OnDisable()
        {
            Tweaker.OnChangeSpeedButtonClick -= HandleChangeSpeedButtonClick;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _anim.SetTrigger(_nameAnimation);
            _anim.speed = _speedButtonClick;
        }

        public void HandleButtonClick() => EventButtonClick?.Invoke();

        private void HandleChangeSpeedButtonClick(float speedButtonClick) => _speedButtonClick = speedButtonClick;
    }
}