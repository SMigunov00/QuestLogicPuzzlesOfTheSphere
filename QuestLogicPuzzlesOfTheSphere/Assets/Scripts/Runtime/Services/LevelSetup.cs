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
        [field: SerializeField] public float StepsToGetAStar { get; private set; }
        [field: SerializeField] public int CurrentCountStarsOnTheLevel { get; private set; }
        [field: SerializeField] public LayerMask SelectableObjectMask { get; private set; }

        public ActivablePoint[] ActivablePoints { get; private set; }
        public int MaxCountOfItemsRaised { get; private set; }
        public int RequiredCountToWin { get; private set; }

        private void Awake()
        {
            ActivablePoints = FindObjectsOfType<ActivablePoint>();
            MaxCountOfItemsRaised = FindObjectsOfType(typeof(PickableObject)).Length;
            RequiredCountToWin = ActivablePoints.Length - 1;
        }
    }
}