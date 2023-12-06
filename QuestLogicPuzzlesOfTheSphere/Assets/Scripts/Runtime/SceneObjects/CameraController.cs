using UnityEngine;
using Runtime.Save;
using Cinemachine;

namespace Runtime.SceneObjects
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _orthographicCamera;
        [SerializeField] private CinemachineVirtualCamera _perspectiveCamera;

        private CinemachineVirtualCamera _activeCamera;

        private GameSettingsData _settingsData = new GameSettingsData();

        private void OnEnable()
        {
            Tweaker.OnToggleCamera += HandleSwitch;
        }

        private void OnDisable()
        {
            Tweaker.OnToggleCamera -= HandleSwitch;
        }

        private void Awake()
        {
            _settingsData = JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString("SavedGameSettingsData"));
        }

        private void Start()
        {
            _activeCamera = _perspectiveCamera;
            HandleSwitch(_settingsData.IsOrthographicCamera);
        }

        private void HandleSwitch(bool isOrthographicCamera)
        {
            _activeCamera.Priority = 0;

            if (isOrthographicCamera)
                _activeCamera = _orthographicCamera;
            else
                _activeCamera = _perspectiveCamera;

            _activeCamera.Priority = 10;
        }
    }
}