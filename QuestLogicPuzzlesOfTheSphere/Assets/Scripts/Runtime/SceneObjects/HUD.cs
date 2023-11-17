using Runtime.Services;
using UnityEngine;

namespace Runtime.SceneObjects
{
    public class HUD : MonoBehaviour
    {
        [field: SerializeField] public Star StarForSteps { get; private set; }
        [field: SerializeField] public Star StarForRaisedObjects { get; private set; }
        [field: SerializeField] public Star StarForTime { get; private set; }
        [field: SerializeField] public Timer Timer { get; private set; }
    }
}