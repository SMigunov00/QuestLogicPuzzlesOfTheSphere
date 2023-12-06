using Runtime.Save;
using Runtime.Services;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Runtime.SceneObjects
{
    public class Lane : MonoBehaviour, IActivable
    {

        [SerializeField] private GameObject _energyLine;
        [SerializeField] private float _speedActivatedEnergyLine = 0.1f;
        [SerializeField] private bool _initializeCristalGroup;
        [SerializeField] private GameObject _cristalGroup;
        [SerializeField] private Transform _firstRaycastPoint;
        [SerializeField] private Transform _secondRaycastPoint;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _lenghtRay;

        private Vector2 _outMinMax;

        private float _maxEnergy = 250f;
        private Color _color = Color.white;
        private GameSettingsData _settingsData = new GameSettingsData();

        private PlayerMover _playerMover;
        public bool IsActive { get; private set; }

        [Inject]
        private void Constructo(LevelSetup levellSetup, PlayerMover playerMover)
        {
            _color = levellSetup.LevelColor;
            _playerMover = playerMover;
        }

        private void OnValidate()
        {
            _cristalGroup.SetActive(_initializeCristalGroup);
            InactiveGlow();
        }

        private void Start()
        {
            Tweaker.OnChangeGlowPower += HandleChangeGlowPower;
            _settingsData = JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString("SavedGameSettingsData"));
            _maxEnergy = _settingsData.MaxGlowPower;
            _cristalGroup.SetActive(_initializeCristalGroup);
            InactiveGlow();
        }

        private void Update()
        {
            if (!IsActive)
            {
                RaycastHit hit;
                Ray firstRay = new Ray(_firstRaycastPoint.position, transform.up);
                bool firstIntersect = Physics.Raycast(firstRay, out hit, _lenghtRay, _layerMask);
                Ray secondRay = new Ray(_secondRaycastPoint.position, transform.up);
                bool secondIntersect = Physics.Raycast(secondRay, out hit, _lenghtRay, _layerMask);
                if (firstIntersect)
                    _outMinMax = new Vector2(0, 1);
                else if (secondIntersect)
                    _outMinMax = new Vector2(1, 0);

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
            {
                UpdateLaneEmissionColor(newEnergy, 1f);
                UpdateEnergyLineEmissionColor(newEnergy, 1f);
            }
        }

        private void InactiveGlow()
        {
            UpdateLaneEmissionColor(_maxEnergy, 0f);
            UpdateEnergyLineEmissionColor(_maxEnergy, 0f);
            _energyLine.SetActive(false);
        }

        public void ActivateObject()
        {
            if (!IsActive)
            {
                IsActive = true;

                StartCoroutine(UpdateLaneEmission(_maxEnergy));
            }
        }

        float _glowPower;
        private IEnumerator UpdateLaneEmission(float currentEnergy)
        {
            float i = 0f;
            while (i <= 1f)
            {
                i += _playerMover.Player.MoveSpeed / 10 * Time.deltaTime;
                UpdateLaneEmissionColor(currentEnergy, i);
                yield return null;
            }
            StartCoroutine(UpdateEnergyLineEmission(currentEnergy));
        }

        private IEnumerator UpdateEnergyLineEmission(float currentEnergy)
        {
            float i = 0f;
            _energyLine.SetActive(true);
            while (i < 1f)
            {
                i += _speedActivatedEnergyLine;
                UpdateEnergyLineEmissionColor(currentEnergy, i);
                yield return null;
            }
        }

        private void UpdateEnergyLineEmissionColor(float energy, float treshold)
        {
            UpdateGlowPower(energy);

            Renderer[] elements = _energyLine.GetComponents<Renderer>();
            foreach (Renderer element in elements)
            {
                UpdateEnergyLineEmissionColor(element, _glowPower, treshold);
            }
        }

        private void UpdateLaneEmissionColor(float energy, float treshold)
        {
            UpdateGlowPower(energy);

            Renderer[] elements = GetComponentsInChildren<Renderer>();
            foreach (Renderer element in elements)
            {
                UpdateLaneEmissionColor(element, _glowPower, treshold);
            }
        }

        private void UpdateGlowPower(float energy)
        {
            float glowPower = energy / 100.0f;

            float k = 1.442695f;
            _glowPower = Mathf.Log(glowPower + 1) * k;
        }

        private void UpdateEnergyLineEmissionColor(Renderer element, float glowPower, float treshold)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            element.GetPropertyBlock(propBlock);
            propBlock.SetColor("_FresnelEnergyLaneColor", _color * glowPower);
            propBlock.SetFloat("_Treshold", treshold);
            element.SetPropertyBlock(propBlock);
        }

        private void UpdateLaneEmissionColor(Renderer element, float glowPower, float treshold)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            element.GetPropertyBlock(propBlock);
            propBlock.SetVector("_LaneOutMinMax", _outMinMax);
            propBlock.SetColor("_FresnelLaneColor", _color * glowPower);
            propBlock.SetColor("_EmitTextureColor", _color * glowPower);
            propBlock.SetFloat("_Treshold", treshold);
            element.SetPropertyBlock(propBlock);
        }
    }
}