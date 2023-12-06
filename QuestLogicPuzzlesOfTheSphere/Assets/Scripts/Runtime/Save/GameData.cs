using System.Collections.Generic;

namespace Runtime.Save
{
    [System.Serializable]
    public class GameData
    {
        public int RisedObjectScore;
        public int CountUnlockedLevel = 2;
        public List<float> TransitTime = new List<float>();
        public List<int> NumberOfStarsOnALevel = new List<int>();
    }
}