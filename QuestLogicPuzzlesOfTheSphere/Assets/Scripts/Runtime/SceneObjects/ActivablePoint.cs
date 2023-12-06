using Runtime.Save;
using Runtime.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Runtime.SceneObjects
{
    public class ActivablePoint : SelectableObject, IActivable
    {
        [field: SerializeField] public bool IsStartPoint { get; private set; }
        [SerializeField] private Transform _positionPoint;
        private int _numberOfPaths;
        protected float _maxEnergy = 250f;
        private Color _color = Color.white;

        private float _currentEnergy;
        private float _deltaStepsUpdateEmission;

        protected ActivatedPointCounter _activatedPointCounter;
        private GameState _gameState;

        protected bool _isTransparentLaneBeingActivated;

        public bool ObjectActivates { get; protected set; }

        public override Vector3 GetPointPosition() => _positionPoint.position;
        public override List<Lane> GetDirections() => _availableDestinations;
        public override int GetCountFried() => _availableDestinations.Count;

        [Inject]
        private void Constructor(ActivatedPointCounter activatedPointCounter, LevelSetup levelSetup, GameState gameState)
        {
            _activatedPointCounter = activatedPointCounter;
            _color = levelSetup.LevelColor;
            _gameState = gameState;
        }

        public bool IsActive { get; protected set; }

        private void OnValidate()
        {
            CalculateNumberOfPaths();
            UpdateGlow();
        }

        private void Start()
        {
            Tweaker.OnChangeGlowPower += HandleChangeGlowPower;
            GameSettingsData settingsData = JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString("SavedGameSettingsData"));
            _maxEnergy = settingsData.MaxGlowPower;
            CalculateNumberOfPaths();
            UpdateGlow();
        }

        private void OnDisable()
        {
            Tweaker.OnChangeGlowPower -= HandleChangeGlowPower;
        }

        public void HandleChangeGlowPower(float newEnergy)
        {
            _maxEnergy = newEnergy;
            if (IsActive || IsStartPoint)
            {
                _glowPower = 0f;
                _deltaStepsUpdateEmission = _maxEnergy / _numberOfPaths;
                UpdateEmissionColor(_deltaStepsUpdateEmission);
            }
        }

        private void UpdateGlow()
        {
            _deltaStepsUpdateEmission = _maxEnergy / _numberOfPaths;
            if (IsStartPoint)
                UpdateEmissionColor(_maxEnergy);
            else
                UpdateEmissionColor(0f);
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

        [SerializeField] protected float _inspectionRadius = 6f;
        private void CalculateNumberOfPaths()
        {
            var lanesCollider = Physics.OverlapSphere(transform.position, _inspectionRadius, _interactionRaycastMask);
            _numberOfPaths = lanesCollider.Length;
        }


        private List<bool> _laneFlagsList = new List<bool>();
        private int _numberActivatedPaths = 0;

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
                        Lane lane = hitCollider.collider.gameObject.GetComponent<Lane>();
                        if (!_lanes.Contains(lane))
                        {
                            _lanes.Add(lane);

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
                        _laneFlagsList.Add(true);

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
                _numberActivatedPaths = _laneFlagsList.Count;
                if (_numberActivatedPaths >= _numberOfPaths)
                    _gameState.GameLose("ReasonLosingPointWithNoWayOut");
                else
                    _laneFlagsList.Clear();
            }
        }

        public void RemoveSelectedObjectAtList(Lane lane)
        {
            if (_availableDestinations.Contains(lane))
                _availableDestinations.Remove(lane);
        }


        protected float _glowPower;

        protected IEnumerator UpdateEmission(float currentEnergy)
        {
            ObjectActivates = true;
            while (_glowPower <= currentEnergy)
            {
                _glowPower += 10f;
                UpdateEmissionColor(_glowPower);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            ObjectActivates = false;
        }

        protected void UpdateEmissionColor(float energy)
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
            propBlock.SetColor("_FresnelLaneColor", _color * glowPower);
            propBlock.SetColor("_EmissionColor", _color * glowPower);
            element.SetPropertyBlock(propBlock);
        }
    }
}
