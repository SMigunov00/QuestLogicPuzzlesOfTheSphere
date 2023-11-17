using Runtime.Interfaces;
using UnityEngine;

namespace Runtime.SceneObjects
{
    public class PickableObject : MonoBehaviour, IPickable
    {
        [SerializeField] private float _rotateSpeed = 25f;
        public void PickUp()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.Rotate(0f, _rotateSpeed * Time.deltaTime, 0f);
        }
    }
}