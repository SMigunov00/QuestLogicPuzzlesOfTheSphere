using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Runtime.SceneObjects
{
    public class ActivablePointWithTransparentPaths : ActivablePoint
    {
        [SerializeField] private PointTypes _pointType;

        private List<ActivablePoint> _activablePointsList = new List<ActivablePoint>();
        private List<TransparentLane> _receivingLaneList = new List<TransparentLane>();
        private List<TransparentLane> _energizingLaneList = new List<TransparentLane>();
        private List<TransparentLane> _bidirectionalLaneList = new List<TransparentLane>();

        [Inject] private ActivableObjectActivator _objectActivator;


        protected override void Awake()
        {
            _objectActivator.AddPointWithTransparentPaths(this);
            FindActivablePointsAround();
        }

        private void Update()
        {
            if (!IsActive)
            {
                switch (_pointType)
                {
                    case PointTypes.WithoutPaths:
                        if (_activablePointsList.All(x => x.IsActive))
                            ActivateObject();
                        break;

                    case PointTypes.Redistributiveble:
                        if (_energizingLaneList.All(x => x.IsActive))
                        {
                            ActivateObject();
                            foreach (var lane in _receivingLaneList)
                            {
                                if (!lane.IsActive)
                                    lane.HandleActivatedLane();
                            }
                        }
                        break;

                    case PointTypes.Receiveble:
                        if (_receivingLaneList.All(x => x.IsActive))
                            ActivateObject();
                        break;

                    case PointTypes.Bidirectional:
                        if (_bidirectionalLaneList.Any(x => x.IsActive))
                        {
                            ActivateObject();
                            foreach (var lane in _bidirectionalLaneList)
                            {
                                if (!lane.IsActive)
                                    lane.HandleActivatedLane();
                            }
                        }
                        break;
                }
            }
        }

        private new void ActivateObject()
        {
            IsActive = true;
            _activatedPointCounter.CalculateActivatedPoint(this);
            StartCoroutine(UpdateEmission(_maxEnergy));
        }

        private void FindActivablePointsAround()
        {
            Collider[] activablePoints = Physics.OverlapSphere(transform.position, _inspectionRadius, _interactionRaycastMask);

            switch (_pointType)
            {
                case PointTypes.WithoutPaths:
                    foreach (Collider point in activablePoints)
                    {
                        if (point.GetComponent<ActivablePoint>()
                            && point.gameObject != gameObject)
                            _activablePointsList.Add(point.GetComponent<ActivablePoint>());
                    }
                    break;

                case PointTypes.Redistributiveble:
                    foreach (Collider point in activablePoints)
                    {
                        var lane = point.GetComponent<TransparentLane>();
                        if (lane != null)
                        {
                            if (!lane.IsEnergizing || lane.IsReceiving)
                                _receivingLaneList.Add(lane);
                            else
                                _energizingLaneList.Add(lane);
                        }
                    }
                    break;

                case PointTypes.Receiveble:
                    foreach (Collider point in activablePoints)
                    {
                        var lane = point.GetComponent<TransparentLane>();
                        if (lane != null)
                        {
                            if (lane.IsReceiving)
                                _receivingLaneList.Add(lane);
                        }
                    }
                    break;

                case PointTypes.Bidirectional:
                    foreach (Collider point in activablePoints)
                    {
                        var lane = point.GetComponent<TransparentLane>();
                        if (lane != null)
                        {
                            if (lane.IsBidirectional)
                                _bidirectionalLaneList.Add(lane);
                        }
                    }
                    break;
            }

        }

        private enum PointTypes
        {
            WithoutPaths,
            Redistributiveble,
            Receiveble,
            Bidirectional
        }
    }
}