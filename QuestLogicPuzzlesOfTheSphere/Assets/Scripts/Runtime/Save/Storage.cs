using Runtime.SceneObjects;
using Runtime.Services;
using System;
using UnityEngine;
using Zenject;

namespace Runtime.Save
{
    [System.Serializable]
    public class Storage : IInitializable, IDisposable, ISaveble, ILoadable
    {
        private int _risedObjectScore;
        private int _countUnlockedLevel;
        private float _transitTime, _oldTransitTime;

        private GameData _data = new GameData();

        private StepReader _stepReader;
        private Timer _timer;
        private ItemPicker _itemPicker;
        private Stargazer _stargazer;
        private VictoryMenu _victoryMenu;

        [Inject]
        private void Constructor(StepReader stepReader, Timer timer, ItemPicker itemPicker, Stargazer stargazer)
        {
            _stepReader = stepReader;
            _timer = timer;
            _itemPicker = itemPicker;
            _stargazer = stargazer;
        }

        public void Initialize()
        {
            LoadGameData();
            _itemPicker.OnObjectsRaisedScore += SetRisedObjectScore;
        }

        public void Dispose()
        {
            _itemPicker.OnObjectsRaisedScore -= SetRisedObjectScore;
        }

        public void SaveGameData(int indexLevel, int countUnlockLevel)
        {
            if (_data.CountUnlockedLevel <= countUnlockLevel)
                _data.CountUnlockedLevel = countUnlockLevel;

            _data.RisedObjectScore += _risedObjectScore;
            SetTransitTime(indexLevel, _timer.CurrentTime);
            SetNumberOfStarsOnALevel(indexLevel, _stargazer.CurrentCountStarsOnTheLevel);
            PlayerPrefs.SetString("SavedGameData", JsonUtility.ToJson(_data));
        }

        public void LoadGameData()
        {
            if (PlayerPrefs.HasKey("SavedGameData"))
            {
                _data = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString("SavedGameData"));
            }
        }

        public void SetÑountUnlockedLevel(int countUnlockedLevel) => _countUnlockedLevel = countUnlockedLevel;
        public void SetRisedObjectScore(int risedObjectScore) => _risedObjectScore = risedObjectScore;
        public void SetNumberOfStarsOnALevel(int levelIndex, int countStars)
        {
            if (_data.NumberOfStarsOnALevel.Count > levelIndex)
            {
                if (_data.NumberOfStarsOnALevel[levelIndex] <= countStars)
                    _data.NumberOfStarsOnALevel[levelIndex] = countStars;
            }
            else
                _data.NumberOfStarsOnALevel.Add(countStars);
        }

        public void SetTransitTime(int levelIndex, float transitTime)
        {
            if (_data.TransitTime.Count > levelIndex)
            {
                if (_data.TransitTime[levelIndex] >= transitTime)
                    _data.TransitTime[levelIndex] = transitTime;
            }
            else
                _data.TransitTime.Add(transitTime);
        }
    }
}