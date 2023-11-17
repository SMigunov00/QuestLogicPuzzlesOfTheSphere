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

        [Inject]
        private void Constructor(StepReader stepReader, Timer timer, ItemPicker itemPicker)
        {
            _stepReader = stepReader;
            _timer = timer;
            _itemPicker = itemPicker;
        }

        private void OnEnable()
        {
            _stepReader.OnStepsCount += UploadStepsCountOnScreenDisplay;
            _timer.OnConvertedTransitTime += UploadTransitTimeOnScreenDisplay;
            _itemPicker.OnObjectsRaisedScore += UploadRisedObjectsScoreOnScreenDisplay;
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

        private void UploadRisedObjectsScoreOnScreenDisplay(int risedObjectsScore)
        {
            if (_risedObjectScoreText != null)
                _risedObjectScoreText.text = risedObjectsScore.ToString();
        }
        private void UploadStepsCountOnScreenDisplay(int stepsCount)
        {
            if (_stepsCountText != null)
                _stepsCountText.text = $"Ходы: {stepsCount}";
        }
    }
}