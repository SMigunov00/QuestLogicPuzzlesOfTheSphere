using Runtime.Services;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Runtime.SceneObjects
{
    public class Lane : MonoBehaviour, IActivable
    {
        [SerializeField] private float _maxEnergy = 250f;
        [SerializeField] private float _activatedSpeed;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private GameObject _energyLine;
        [SerializeField] private bool _initializeCristalGroup;
        [SerializeField] private GameObject _cristalGroup;

        [Inject]
        private void Constructo(LevelSetup levellSetup)
        {
            _maxEnergy = levellSetup.MaxGlowPower;
            _color = levellSetup.LevelColor;
        }

        public bool IsActive { get; private set; }

        private void OnValidate()
        {
            _cristalGroup.SetActive(_initializeCristalGroup);
            InactiveGlow();
        }

        private void Start()
        {
            _cristalGroup.SetActive(_initializeCristalGroup);
            InactiveGlow();
        }

        private void InactiveGlow()
        {
            UpdateEmissionColor(0f);
            _energyLine.SetActive(false);
        }

        public void ActivateObject()
        {
            if (!IsActive)
            {
                IsActive = true;
                _energyLine.SetActive(true);
                StartCoroutine(UpdateEmission(_maxEnergy));
            }
        }

        float _glowPower;
        private IEnumerator UpdateEmission(float currentEnergy)
        {
            while (_glowPower <= currentEnergy)
            {
                _glowPower += _activatedSpeed;
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