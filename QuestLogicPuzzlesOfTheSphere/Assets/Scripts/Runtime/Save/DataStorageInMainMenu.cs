using System;
using UnityEngine;
using Zenject;

namespace Runtime.Save
{
    [System.Serializable]
    public class DataStorageInMainMenu : IInitializable, ISaveble, ILoadable
    {
        private GameSettingsData _settingsData = new GameSettingsData();

        public void Initialize()
        {
            LoadGameSettingsData();
        }

        public void LoadGameData()
        {
            throw new NotImplementedException();
        }

        public void SaveGameData(int indexLevel, int countUnlockLevel)
        {
            throw new NotImplementedException();
        }

        public void SaveGameSettingsData(GameSettingsData gameSettingsData)
        {
            PlayerPrefs.SetString("SavedGameSettingsData", JsonUtility.ToJson(gameSettingsData));
        }

        public void LoadGameSettingsData()
        {
            if (PlayerPrefs.HasKey("SavedGameSettingsData"))
            {
                _settingsData = JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString("SavedGameSettingsData"));
            }
            else
                SaveGameSettingsData(_settingsData);
        }
    }
}
