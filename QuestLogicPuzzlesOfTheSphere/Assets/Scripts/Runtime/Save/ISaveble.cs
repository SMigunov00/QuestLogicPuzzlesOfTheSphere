namespace Runtime.Save
{
    public interface ISaveble
    {
        void SaveGameData(int indexLevel, int countUnlockLevel);
        void SaveGameSettingsData(GameSettingsData gameSettingsData);
    }
}