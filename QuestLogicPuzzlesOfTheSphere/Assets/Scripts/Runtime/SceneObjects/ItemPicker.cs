using Runtime.Interfaces;
using System;
using UnityEngine;

namespace Runtime.SceneObjects
{
    public class ItemPicker : MonoBehaviour
    {
        [field: SerializeField] public int CurrentCountOfItemsRaised { get; private set; }

        [SerializeField] private LayerMask _pickableItemMask;
        [SerializeField] private float _distanceRaiseItems;


        private void Update()
        {
            var colliders = Physics.OverlapSphere(transform.position, _distanceRaiseItems, _pickableItemMask);
            if (colliders != null)
            {
                foreach (var item in colliders)
                {
                    IPickable pickableItem = item.gameObject.GetComponent<IPickable>();
                    pickableItem?.PickUp();
                    CurrentCountOfItemsRaised++;
                    OnObjectsRaisedScore?.Invoke(CurrentCountOfItemsRaised);
                    OnObjectsRaised?.Invoke();
                }
            }
        }

        public event Action OnObjectsRaised;
        public event Action<int> OnObjectsRaisedScore;
    }
}