using Runtime.Save;
using Runtime.SceneObjects;
using Runtime.Services.Pause;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Runtime.Services
{
    public class VictoryMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _transitTimeText;
        [SerializeField] private TextMeshProUGUI _raisedObjectText;

        public Star[] Stars { get; private set; }

        private int _currentLevel, _countUnlockLevel;

        private ISaveble _saver;

        private HUD _hud;
        private ItemPicker _itemPicker;
        private Timer _timer;
        private IPauseHandler _pauseHandler;

        [Inject]
        private void Constructor(ISaveble saver, HUD hud, ItemPicker itemPicker, Timer timer, IPauseHandler pauseHandler)
        {
            _saver = saver;
            _hud = hud;
            _itemPicker = itemPicker;
            _timer = timer;
            _pauseHandler = pauseHandler;
        }

        private void Awake()
        {
            _timer.OnConvertedTransitTime += SetTransitTime;
            _currentLevel = SceneManager.GetActiveScene().buildIndex - 1;
            _countUnlockLevel = _currentLevel + 3;
            Stars = transform.GetComponentsInChildren<Star>();
        }

        private void OnDisable()
        {
            _timer.OnConvertedTransitTime -= SetTransitTime;
        }

        public void HandleActivatedMenu(int countActiveStars)
        {
            _hud.gameObject.SetActive(false);
            gameObject.SetActive(true);
            SetActivateStars(countActiveStars);
            SetCountRaisedObject();
            _saver.SaveGameData(_currentLevel, _countUnlockLevel);
            _pauseHandler.SetPaused(true);
        }

        private void SetActivateStars(int countActiveStars)
        {
            for (int i = 0; i < countActiveStars; i++)
            {
                Stars[i].SetStateStar(true);
            }
        }

        private void SetTransitTime(int seconds, int minutes, int hours)
        {
            _transitTimeText.text = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
        }

        private void SetCountRaisedObject()
        {
            _raisedObjectText.text = _itemPicker.CurrentCountOfItemsRaised.ToString();
        }
    }
}