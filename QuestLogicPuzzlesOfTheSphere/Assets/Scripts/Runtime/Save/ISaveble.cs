using Runtime.SceneObjects;
using System;

namespace Runtime.Save
{
    public interface ISaveble
    {
        void SaveGameData(int indexLevel, int countUnlockLevel);
    }
}