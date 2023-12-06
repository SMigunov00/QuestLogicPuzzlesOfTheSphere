using Runtime.Localization;
using Runtime.Services;
using Runtime.Services.Pause;
using TMPro;
using UnityEngine;
using Zenject;

namespace Runtime.SceneObjects.Training
{
    public class Educator : MonoBehaviour
    {
        [SerializeField] private GameObject _educatorPanel;
        [SerializeField] private TextMeshProUGUI _learningUIText;
        [SerializeField] private float _lenghtRay;
        [SerializeField] private LayerMask _teleportedObjectMask;
        [SerializeField] private string _key;

        private bool _isLearnedShow;

        private IPauseHandler _pause;
        private IInput _input;

        private LevelSetup _levelSetup;
        private Player _player;
        private PlayerMover _playerMover;
        private Localizator _localizator;

        [Inject]
        private void Constructor(LevelSetup levelSetup, IPauseHandler pause, IInput input, PlayerMover playerMover, Localizator localizator)
        {
            _levelSetup = levelSetup;
            _pause = pause;
            _input = input;
            _playerMover = playerMover;
            _localizator = localizator;
        }

        private void OnEnable()
        {
            _input.OnClick += OnClick;
        }

        private void OnClick(Vector3 position) => TapToScreen();

        private void Update()
        {
            if (!_pause.IsPaused)
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, transform.up);
                bool intersect = Physics.Raycast(ray, out hit, _lenghtRay, _teleportedObjectMask);
                if (intersect && _levelSetup.IsLearningLevel && _playerMover.Coroutine == null)
                {
                    if (!_isLearnedShow)
                    {
                        _isLearnedShow = true;
                        _educatorPanel.SetActive(true);
                        _learningUIText.text = _localizator.GetLocalizedValue(_key);
                        _player ??= hit.collider.gameObject.GetComponent<Player>();
                        _pause.SetPaused(true);
                    }
                }
            }
        }

        private void TapToScreen()
        {
            if (_player != null)
            {
                _educatorPanel.SetActive(false);
                _learningUIText.text = null;
                _player = null;
                _pause.SetPaused(false);
                _input.OnClick -= OnClick;
            }
        }
    }
}