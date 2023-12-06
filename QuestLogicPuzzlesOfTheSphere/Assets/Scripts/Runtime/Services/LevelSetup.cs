using Runtime.SceneObjects;
using UnityEngine;

namespace Runtime.Services
{
    public class LevelSetup : MonoBehaviour
    {
        [field: SerializeField] public bool IsLearningLevel { get; private set; }
        [field: SerializeField] public Color LevelColor { get; private set; }
        [field: SerializeField] public float MaxGlowPower { get; private set; }
        [field: SerializeField] public float TimeToGetAStar { get; private set; }
        [field: SerializeField] public int StepsToGetAStar { get; private set; }
        [field: SerializeField] public int CurrentCountStarsOnTheLevel { get; private set; }
        [field: SerializeField] public LayerMask SelectableObjectMask { get; private set; }

        public ActivablePoint[] ActivablePoints { get; private set; }
        [field: SerializeField] public int TotalNumberOfObjectsToBeCollected { get; private set; }
        [field: SerializeField] public int RequiredNumberOfCollectedObjectsToGetAStar { get; private set; }
        public int RequiredCountToWin { get; private set; }

        private void Awake()
        {
            ActivablePoints = FindObjectsOfType<ActivablePoint>();
            TotalNumberOfObjectsToBeCollected = FindObjectsOfType(typeof(PickableObject)).Length;

            RequiredCountToWin = ActivablePoints.Length - 1;
            CalculateRequiredNumberOfCollectedObjectsToGetAStar();
        }
        
        private void CalculateRequiredNumberOfCollectedObjectsToGetAStar()
        {
            for (int i = 1; i < TotalNumberOfObjectsToBeCollected; i++)
            {
                if ((float)i / TotalNumberOfObjectsToBeCollected >= 0.75f)
                {
                    RequiredNumberOfCollectedObjectsToGetAStar = i;
                    return;
                }
            }
        }
    }
}