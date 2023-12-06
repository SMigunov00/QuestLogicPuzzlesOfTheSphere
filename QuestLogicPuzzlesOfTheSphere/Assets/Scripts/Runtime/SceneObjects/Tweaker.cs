using Runtime.Save;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.SceneObjects
{
    public class Tweaker : MonoBehaviour
    {
        public static event Action<bool> OnToggleCamera;
        public static event Action<float> OnChangeGlowPower;
        public static event Action<float> OnChangeSpeedButtonClick;

        [Inject] private ISaveble _saver;

        private GameSettingsData _settingsData = new GameSettingsData();

        [SerializeField] private Slider _sliderSpeedButtonClick;
        [SerializeField] private Slider _sliderGlowPower;
        [SerializeField] private Color _buttonToggleCameraColor;
        [SerializeField] private GameObject _centerElementButtonToggleCamera;

        private void Awake()
        {
            if (!PlayerPrefs.HasKey("SavedGameSettingsData"))
                PlayerPrefs.SetString("SavedGameSettingsData", JsonUtility.ToJson(_settingsData));

            _settingsData = JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString("SavedGameSettingsData"));
        }

        private void Start()
        {
            _sliderSpeedButtonClick.value = _settingsData.SpeedButtonCLick;
            _sliderGlowPower.value = _settingsData.MaxGlowPower;

            if (_settingsData.IsOrthographicCamera)
                UpdateEmissionColor(_sliderGlowPower.value);
            else
                UpdateEmissionColor(0f);
        }


        public void HandleToggleCamera()
        {
            _settingsData.IsOrthographicCamera = !_settingsData.IsOrthographicCamera;
            _saver?.SaveGameSettingsData(_settingsData);
            OnToggleCamera?.Invoke(_settingsData.IsOrthographicCamera);

            if (_settingsData.IsOrthographicCamera)
                UpdateEmissionColor(_sliderGlowPower.value);
            else
                UpdateEmissionColor(0f);
        }

        public void HandleChangeGlowPower()
        {
            _settingsData.MaxGlowPower = _sliderGlowPower.value;
            _saver?.SaveGameSettingsData(_settingsData);
            OnChangeGlowPower?.Invoke(_settingsData.MaxGlowPower);
            if (_settingsData.IsOrthographicCamera)
                UpdateEmissionColor(_sliderGlowPower.value);
        }

        public void HandleChangeSpeedButtonClick()
        {
            _settingsData.SpeedButtonCLick = _sliderSpeedButtonClick.value;
            _saver?.SaveGameSettingsData(_settingsData);
            OnChangeSpeedButtonClick?.Invoke(_settingsData.SpeedButtonCLick);
        }

        private void UpdateEmissionColor(float energy)
        {
            float glowPower = energy / 100.0f;

            float k = 1.442695f;
            glowPower = Mathf.Log(glowPower + 1) * k;

            Renderer element = _centerElementButtonToggleCamera.GetComponent<Renderer>();
            UpdateEmissionColor(element, glowPower);

        }

        private void UpdateEmissionColor(Renderer element, float glowPower)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            element.GetPropertyBlock(propBlock);
            propBlock.SetColor("_EmissionColor", _buttonToggleCameraColor * glowPower);
            element.SetPropertyBlock(propBlock);
        }
    }
}