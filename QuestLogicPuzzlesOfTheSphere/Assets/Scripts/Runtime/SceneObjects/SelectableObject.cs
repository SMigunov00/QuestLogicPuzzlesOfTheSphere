using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Runtime.SceneObjects
{
    public abstract class SelectableObject : MonoBehaviour
    {
        [SerializeField] protected int _countDirection = 8;
        [SerializeField] protected float _stepRayLenght = 20f;
        [SerializeField] protected float _defaultRayLenght = 3f;
        [SerializeField] protected Transform _raycastPoint;

        [SerializeField] protected LayerMask _interactionRaycastMask;

        protected float _currentRayLenght;

        [SerializeField] protected List<Lane> _availableDestinations = new List<Lane>();
        protected List<Lane> _lanes = new List<Lane>();

        public abstract Vector3 GetPointPosition();
        public abstract void StopAtSelectedPoint(Player player);
        public abstract List<Lane> GetDirections();
        public abstract int GetCountFried();

        protected void Awake()
        {
            _currentRayLenght = _defaultRayLenght;
            FindLaneDirection();
        }

        protected int i;
        bool laneActivated;
        [SerializeField] protected List<bool> _checkedLanesFlag = new List<bool>();
        [SerializeField] protected List<Lane> _checkedLanes = new List<Lane>();
        public bool CheckLaneActivated(Vector3 direction, Player player)
        {
            _raycastPoint.LookAt(direction);
            float rayLenght = 0;
            if (player.CountMovedActivatedLane == 0)
                rayLenght = (transform.position - direction).magnitude;
            else
                rayLenght = (transform.position - direction).magnitude + _stepRayLenght;
            Ray ray = new Ray(transform.position, _raycastPoint.forward);
            var hitColliders = Physics.RaycastAll(ray, rayLenght, _interactionRaycastMask);
            Debug.Log(hitColliders.Length);
            if (hitColliders.Length > _checkedLanes.Count)
            {
                foreach (var hitCollider in hitColliders)
                {
                    Lane lane = hitCollider.collider.gameObject.GetComponent<Lane>();

                    if (!_checkedLanes.Contains(lane))
                    {
                        _checkedLanes.Add(lane);
                        _checkedLanesFlag.Add(lane.IsActive);
                    }

                }
                i = 0;
                foreach (bool activatedLaneFlag in _checkedLanesFlag)
                {
                    if (activatedLaneFlag)
                        i++;
                }

                if (i <= player.CountMovedActivatedLane)
                    laneActivated = false;
                else
                    laneActivated = true;

                _checkedLanes.Clear();
                _checkedLanesFlag.Clear();

            }
            return laneActivated;
        }

        protected void FindLaneDirection()
        {
            if (i < _countDirection)
            {
                Ray ray = new Ray(transform.position, transform.forward);
                var hitColliders = Physics.RaycastAll(ray, _currentRayLenght, _interactionRaycastMask);
                if (hitColliders.Length > _lanes.Count)
                {
                    foreach (var hitCollider in hitColliders)
                    {
                        Lane lane = hitCollider.collider.gameObject.GetComponent<Lane>();
                        if (!_availableDestinations.Contains(lane))
                        {
                            _lanes.Add(lane);
                            _availableDestinations.Add(lane);
                        }
                    }
                    _currentRayLenght += _stepRayLenght;
                    FindLaneDirection();
                }
                else
                {
                    i++;
                    _lanes.Clear();
                    _currentRayLenght = _defaultRayLenght;
                    transform.Rotate(0, 45f, 0);
                    FindLaneDirection();
                }
            }
            else
            {
                i = 0;
                transform.Rotate(0, 0, 0);
            }
        }
    }
}