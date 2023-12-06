using Runtime.SceneObjects;
using Runtime.Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace Runtime.UI
{
    public class GameProgressScreenDisplayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _stepsCountText;
        [SerializeField] private TextMeshProUGUI _transitTimeText;
        [SerializeField] private TextMeshProUGUI _risedObjectScoreText;

        private StepReader _stepReader;
        private Timer _timer;
        private ItemPicker _itemPicker;
        private LevelSetup _levelSetup;

        [Inject]
        private void Constructor(StepReader stepReader, Timer timer, ItemPicker itemPicker, LevelSetup levelSetup)
        {
            _stepReader = stepReader;
            _timer = timer;
            _itemPicker = itemPicker;
            _levelSetup = levelSetup;
        }

        private void OnEnable()
        {
            _stepReader.OnStepsCount += UploadStepsCountOnScreenDisplay;
            _timer.OnConvertedTransitTime += UploadTransitTimeOnScreenDisplay;
            _itemPicker.OnObjectsRaisedScore += UploadRisedObjectsScoreOnScreenDisplay;
        }

        private void Start()
        {
            UploadRisedObjectsScoreOnScreenDisplay(0);
        }

        private void OnDisable()
        {
            _stepReader.OnStepsCount -= UploadStepsCountOnScreenDisplay;
            _timer.OnConvertedTransitTime -= UploadTransitTimeOnScreenDisplay;
            _itemPicker.OnObjectsRaisedScore -= UploadRisedObjectsScoreOnScreenDisplay;
        }

        private void UploadTransitTimeOnScreenDisplay(int seconds, int minutes, int hours)
        {
            if (_transitTimeText != null)
                _transitTimeText.text = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
        }

        private void UploadRisedObjectsScoreOnScreenDisplay(int currentNumberOfCollectedObjectsToGetAStar)
        {
            if (_risedObjectScoreText != null)
                _risedObjectScoreText.text = $":{currentNumberOfCollectedObjectsToGetAStar}|{_levelSetup.RequiredNumberOfCollectedObjectsToGetAStar}";
        }
        private void UploadStepsCountOnScreenDisplay(int numberOfStepsUntilTheStarIsDeactivated, int numberOfStepsToLose)
        {
            if (_stepsCountText != null)
                _stepsCountText.text = $"{numberOfStepsUntilTheStarIsDeactivated}|{numberOfStepsToLose}";
        }
    }
}