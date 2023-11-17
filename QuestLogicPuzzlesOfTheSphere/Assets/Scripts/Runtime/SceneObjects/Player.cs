using Runtime.Interfaces;
using UnityEngine;

namespace Runtime.SceneObjects
{
    public class Player : MonoBehaviour, IDamagable
    {
        [SerializeField] private int _health;
        [field: SerializeField] public int CountMovedActivatedLane { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float RotateSpeed { get; private set; }

        public void SetDamage()
        {
            _health--;
            if (_health <= 0)
                gameObject.SetActive(false);
        }
    }
}