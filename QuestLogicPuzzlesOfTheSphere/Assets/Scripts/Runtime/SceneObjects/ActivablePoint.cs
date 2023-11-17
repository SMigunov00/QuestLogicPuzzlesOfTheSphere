using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Zenject;
using Runtime.Services;
using System.Linq;

namespace Runtime.SceneObjects
{
    public class ActivablePoint : SelectableObject, IActivable
    {
        [SerializeField] private int _countDestinations;
        [field: SerializeField] public bool IsStartPoint { get; private set; }
        [SerializeField] private Transform _positionPoint;
        [SerializeField] private float _maxEnergy = 250f;
        [SerializeField] private Color _color = Color.white;

        private float _currentEnergy;
        [SerializeField] private float _deltaStepsUpdateEmission;

        private ActivatedPointCounter _activatedPointCounter;
        private GameState _gameState;

        [Inject]
        private void Constructor(ActivatedPointCounter activatedPointCounter, LevelSetup levellSetup, GameState gameState)
        {
            _activatedPointCounter = activatedPointCounter;
            _maxEnergy = levellSetup.MaxGlowPower;
            _color = levellSetup.LevelColor;
            _gameState = gameState;
        }

        public bool IsActive { get; private set; }

        private void OnValidate()
        {
            CalculateCountDestinations();
            UpdateGlow();
        }

        private void Start()
        {
            CalculateCountDestinations();
            UpdateGlow();
        }

        private void UpdateGlow()
        {
            _deltaStepsUpdateEmission = _maxEnergy / _countDestinations;
            if (IsStartPoint)
                UpdateEmissionColor(_maxEnergy);
            else
                UpdateEmissionColor(0f);
        }

        public override Vector3 GetPointPosition()
        {
            return _positionPoint.position;
        }

        public void ActivateObject()
        {
            if (_availableDestinations != null && !IsStartPoint)
            {
                _currentEnergy += _deltaStepsUpdateEmission;
                if (_currentEnergy < _maxEnergy)
                    StartCoroutine(UpdateEmission(_currentEnergy));
                else
                    StartCoroutine(UpdateEmission(_maxEnergy));

                if (!IsActive)
                {
                    IsActive = true;
                    _activatedPointCounter.CalculateActivatedPoint(this);
                }
            }
        }

        [SerializeField] private float _radiusCheked = 6f;
        private void CalculateCountDestinations()
        {
            var colliders = Physics.OverlapSphere(transform.position, _radiusCheked, _interactionRaycastMask);
            _countDestinations = colliders.Length;
        }


        [SerializeField] List<bool> newList = new List<bool>();
        int j = 0;

        public override void StopAtSelectedPoint(Player player)
        {
            if (i < _countDirection)
            {
                Ray ray = new Ray(transform.position, _positionPoint.forward);
                var hitColliders = Physics.RaycastAll(ray, _currentRayLenght, _interactionRaycastMask);
                if (hitColliders.Length > _lanes.Count && _lanes.Count <= player.CountMovedActivatedLane)
                {
                    foreach (var hitCollider in hitColliders)
                    {
                        Lane lanes = hitCollider.collider.gameObject.GetComponent<Lane>();
                        if (!_lanes.Contains(lanes))
                        {
                            _lanes.Add(lanes);

                        }
                    }
                    _currentRayLenght += _stepRayLenght;
                    StopAtSelectedPoint(player);
                }
                else
                {
                    int x = 0;
                    foreach (var lane in _lanes)
                    {
                        if (lane.IsActive)
                            x++;
                    }
                    if (x > player.CountMovedActivatedLane)
                        newList.Add(true);

                    i++;
                    _lanes.Clear();
                    _currentRayLenght = _defaultRayLenght;
                    _positionPoint.Rotate(0, 45f, 0);
                    StopAtSelectedPoint(player);
                }
            }
            else
            {
                i = 0;
                _positionPoint.Rotate(0, 0, 0);
                j = newList.Count;
                if (j >= _countDestinations)
                    _gameState.GameLose("¬ы встали на точку, из которой нет выхода");
                else
                    newList.Clear();
            }
            /* if (_availableDestinations != null)
             {
                 _currentEnergy += _deltaStepsUpdateEmission;
                 if (_currentEnergy < _maxEnergy)
                     StartCoroutine(UpdateEmission(_currentEnergy));
                 else
                     StartCoroutine(UpdateEmission(_maxEnergy));
                 //lane.ActivateObject();
                 //RemoveSelectedObjectAtList(lastSelectableObject);
                 //lastSelectableObject.RemoveSelectedObjectAtList(this);
                 ActivateObject();
                 /*if (!IsActive)
                 {
                     IsActive = true;
                     _activatedPointCounter.CalculateActivatedPoint(this);
                 }*/


        }

        public void RemoveSelectedObjectAtList(Lane lane)
        {
            if (_availableDestinations.Contains(lane))
                _availableDestinations.Remove(lane);
        }

        public override List<Lane> GetDirections()
        {
            return _availableDestinations;
        }

        public override int GetCountFried()
        {
            return _availableDestinations.Count;
        }
        float _glowPower;
        private IEnumerator UpdateEmission(float currentEnergy)
        {
            while (_glowPower <= currentEnergy)
            {
                _glowPower += 10f;
                UpdateEmissionColor(_glowPower);
                yield return null;
            }
        }

        private void UpdateEmissionColor(float energy)
        {
            float glowPower = energy / 100.0f;

            float k = 1.442695f;
            glowPower = Mathf.Log(glowPower + 1) * k;

            Renderer[] elements = GetComponentsInChildren<Renderer>();
            foreach (Renderer element in elements)
            {
                UpdateEmissionColor(element, glowPower);
            }
        }

        private void UpdateEmissionColor(Renderer element, float glowPower)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            element.GetPropertyBlock(propBlock);
            propBlock.SetColor("_FresnelColor", _color * glowPower);
            propBlock.SetColor("_EmissionColor", _color * glowPower);
            element.SetPropertyBlock(propBlock);
        }
    }
}
