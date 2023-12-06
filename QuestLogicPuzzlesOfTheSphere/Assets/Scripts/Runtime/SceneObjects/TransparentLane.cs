using Runtime.Save;
using Runtime.Services;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Runtime.SceneObjects
{
    public class TransparentLane : MonoBehaviour
    {
        [SerializeField] private float _laneActivatedSpeed = 0.05f;
        private float _maxEnergy = 250f;
        private Color _color = Color.white;

        [SerializeField] private Transform _firstRaycastPoint;
        [SerializeField] private Transform _secondRaycastPoint;
        [SerializeField] private LayerMask _activablePointMask;
        [SerializeField] private float _lenghtRay;

        private Vector2 _outMinMax;

        private ActivableObjectActivator _objectActivator;
        
        [field: SerializeField] public bool IsEnergizing { get; private set; }
        [field: SerializeField] public bool IsReceiving { get; private set; }
        [field: SerializeField] public bool IsBidirectional { get; private set; }

        [Inject]
        private void Constructo(LevelSetup levellSetup, ActivableObjectActivator objectActivator)
        {
            _color = levellSetup.LevelColor;
            _objectActivator = objectActivator;
        }

        public bool IsActive { get; private set; }
        public bool ObjectActivates { get; private set; }

        private void OnValidate()
        {
            InactiveGlow();
        }

        private void Start()
        {
            Tweaker.OnChangeGlowPower += HandleChangeGlowPower;
            _objectActivator.AddTransparentLane(this);
            GameSettingsData settingsData = JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString("SavedGameSettingsData"));
            _maxEnergy = settingsData.MaxGlowPower;
            InactiveGlow();
        }

        private void Update()
        {
            if (!IsActive)
                FindTheActivationSide();

        }

        private void FindTheActivationSide()
        {
            Ray firstRay = new Ray(_firstRaycastPoint.position, _firstRaycastPoint.forward);
            Physics.Raycast(firstRay, out RaycastHit firstHit, _lenghtRay, _activablePointMask);

            Ray secondRay = new Ray(_secondRaycastPoint.position, _secondRaycastPoint.forward);
            Physics.Raycast(secondRay, out RaycastHit secondHit, _lenghtRay, _activablePointMask);

            if (firstHit.collider != null)
            {
                if (firstHit.collider.gameObject.GetComponent<ActivablePoint>().IsActive)
                {
                    _outMinMax = new Vector2(0, 1);
                    ActivateObject();
                }
            }
            if (secondHit.collider != null)
            {
                if (secondHit.collider.gameObject.GetComponent<ActivablePoint>().IsActive)
                {
                    _outMinMax = new Vector2(1, 0);
                    ActivateObject();
                }
            }
        }

        private void OnDisable()
        {
            Tweaker.OnChangeGlowPower -= HandleChangeGlowPower;
        }

        public void HandleChangeGlowPower(float newEnergy)
        {
            _maxEnergy = newEnergy;
            if (IsActive)
                UpdateEmissionColor(newEnergy, 1f);
        }

        private void InactiveGlow() => UpdateEmissionColor(_maxEnergy, 0f);

        public void HandleActivatedLane() => FindTheActivationSide();

        private void ActivateObject()
        {
            if (!IsActive)
                StartCoroutine(UpdateEmission(_maxEnergy));
        }

        private float _glowPower;

        private void UpdateGlowPower(float energy)
        {
            float glowPower = energy / 100.0f;

            float k = 1.442695f;
            _glowPower = Mathf.Log(glowPower + 1) * k;
        }

        private IEnumerator UpdateEmission(float currentEnergy)
        {
            float i = 0f;
            while (i < 1f)
            {
                ObjectActivates = true;
                i += _laneActivatedSpeed;
                UpdateEmissionColor(currentEnergy, i);
                yield return null;
            }
            IsActive = true;
            yield return new WaitForSeconds(0.5f);
            ObjectActivates = false;
        }

        private void UpdateEmissionColor(float energy, float treshold)
        {
            UpdateGlowPower(energy);

            Renderer[] elements = GetComponentsInChildren<Renderer>();
            foreach (Renderer element in elements)
            {
                UpdateEmissionColor(element, treshold);
            }
        }

        private void UpdateEmissionColor(Renderer element, float treshold)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            element.GetPropertyBlock(propBlock);
            propBlock.SetVector("_OutMinMax", _outMinMax);
            propBlock.SetColor("_FresnelLaneColor", _color * _glowPower);
            propBlock.SetFloat("_Treshold", treshold);
            element.SetPropertyBlock(propBlock);
        }
    }
}