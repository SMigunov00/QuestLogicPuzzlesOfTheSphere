using Runtime.SceneObjects;
using Runtime.Services;
using UnityEngine;
using Zenject;

namespace Runtime.Save
{
    [System.Serializable]
    public class Storage : IInitializable, ISaveble, ILoadable
    {
        private GameData _data = new GameData();
        private GameSettingsData _settingsData = new GameSettingsData();

        private Timer _timer;
        private ItemPicker _itemPicker;
        private Stargazer _stargazer;

        [Inject]
        private void Constructor(Timer timer, ItemPicker itemPicker, Stargazer stargazer)
        {
            _timer = timer;
            _itemPicker = itemPicker;
            _stargazer = stargazer;
        }

        public void Initialize()
        {
            LoadGameData();
            LoadGameSettingsData();
        }

        public void SaveGameData(int indexLevel, int countUnlockLevel)
        {
            if (_data.CountUnlockedLevel <= countUnlockLevel)
                _data.CountUnlockedLevel = countUnlockLevel;

            _data.RisedObjectScore += _itemPicker.CurrentCountOfItemsRaised;
            SetTransitTime(indexLevel, _timer.CurrentTime);
            SetNumberOfStarsOnALevel(indexLevel, _stargazer.CurrentCountStarsOnTheLevel);
            PlayerPrefs.SetString("SavedGameData", JsonUtility.ToJson(_data));
            if (!PlayerPrefs.HasKey("OneTimeBonus"))
                PlayerPrefs.SetString("OneTimeBonus", "BonusImposableReceived");
        }

        public void LoadGameData()
        {
            if (PlayerPrefs.HasKey("SavedGameData"))
                _data = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString("SavedGameData"));
        }

        public void SaveGameSettingsData(GameSettingsData gameSettingsData)
        {
            PlayerPrefs.SetString("SavedGameSettingsData", JsonUtility.ToJson(gameSettingsData));
        }

        public void LoadGameSettingsData()
        {
            if (PlayerPrefs.HasKey("SavedGameSettingsData"))
                _settingsData = JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString("SavedGameSettingsData"));
            else
                SaveGameSettingsData(_settingsData);
        }

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