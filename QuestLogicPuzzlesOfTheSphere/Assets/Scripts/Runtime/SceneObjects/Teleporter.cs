using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace Runtime.SceneObjects
{
    public class Teleporter : SelectableObject, IActivable
    {
        [SerializeField] private Teleporter _friend;
        [SerializeField] private Material _teleportMaterial;
        [SerializeField] private Transform _teleportPoint, _positionPoint;
        [SerializeField] private float _lenghtRay;
        [SerializeField] private LayerMask _teleportedObjectMask;

        public bool IsActive { get; private set; }

        public override Vector3 GetPointPosition()
        {
            return _positionPoint.position;
        }

        public override void StopAtSelectedPoint(Player player)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.up);
            bool intersect = Physics.Raycast(ray, out hit, _lenghtRay, _teleportedObjectMask);
            if (intersect)
            {
                StartCoroutine(Teleported(hit.collider.gameObject));
                IsActive = true;
            }
        }

        public SelectableObject GetFriend()
        {
            return _friend.gameObject.GetComponent<SelectableObject>();
        }

        public override List<Lane> GetDirections()
        {
            return _availableDestinations;
        }
        public override int GetCountFried()
        {
            return _availableDestinations.Count;
        }
        /*public override void RemoveSelectedObjectAtList(SelectableObject selectableObject)
        {
            _availableDestinations.Remove(selectableObject);
        }*/

        private Vector3 GetTeleportPoint() => _teleportPoint != null ? _teleportPoint.position : transform.position;

        public void ActivateObject()
        {
            //StartCoroutine(Teleported());
        }

        private IEnumerator Teleported(GameObject hitGameObject)
        {
            Renderer[] elements = hitGameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer element in elements)
            {
                yield return StartCoroutine(UdpateTeleportedEffect(element, 1));
            }
            hitGameObject.transform.position = _friend.GetTeleportPoint();
            foreach (Renderer element in elements)
            {
                yield return StartCoroutine(UdpateTeleportedEffect(element, -1));
            }
        }
        private IEnumerator UdpateTeleportedEffect(Renderer element, int m)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            element.GetPropertyBlock(propBlock);
            Material defaultObjectMaterial = element.material;
            element.material = _teleportMaterial;
            float currentStateEffect = propBlock.GetFloat("_Treshold");
            switch (currentStateEffect)
            {
                case 0f:
                    while (currentStateEffect < 1)
                    {
                        currentStateEffect += Time.deltaTime * m;
                        propBlock.SetFloat("_Treshold", currentStateEffect);
                        element.SetPropertyBlock(propBlock);
                        yield return null;
                    }
                    propBlock.SetFloat("_Treshold", 1f);
                    element.SetPropertyBlock(propBlock);
                    break;
                case 1f:
                    while (currentStateEffect > 0)
                    {
                        currentStateEffect += Time.deltaTime * m;
                        propBlock.SetFloat("_Treshold", currentStateEffect);
                        element.SetPropertyBlock(propBlock);
                        yield return null;
                    }
                    propBlock.SetFloat("_Treshold", 0f);
                    element.SetPropertyBlock(propBlock);
                    break;
            }
            element.material = defaultObjectMaterial;
        }

    }
}