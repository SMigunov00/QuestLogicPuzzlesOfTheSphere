using UnityEngine;

namespace Runtime.SceneObjects
{
    public class Star : MonoBehaviour
    {
        public bool IsChanged { get; private set; }
        [field: SerializeField] public bool IsActive { get; private set; }
        [SerializeField] private Color _color;
        [SerializeField] private float _energy = 500f;

        private Renderer[] _elements;

        private void Awake()
        {
            _elements = GetComponentsInChildren<Renderer>();
        }

        private void OnEnable()
        {
            SetStateStar(IsActive);
        }

        public void ChangeStateStar(bool isActive)
        {
            SetStateStar(isActive);
            IsChanged = !IsChanged;
        }

        public void SetStateStar(bool isActive)
        {
            IsActive = isActive;
            if (gameObject.activeInHierarchy) 
            {
                if (isActive)
                    UpdateEmissionColor(_energy);
                else
                    UpdateEmissionColor(0f);
            }
        }

        private void UpdateEmissionColor(float energy)
        {
            float glowPower = energy / 100.0f;

            float k = 1.442695f;
            glowPower = Mathf.Log(glowPower + 1) * k;

            foreach (Renderer element in _elements)
                UpdateEmissionColor(element, glowPower);
        }

        private void UpdateEmissionColor(Renderer element, float glowPower)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            element.GetPropertyBlock(propBlock);
            propBlock.SetColor("_EmissionColor", _color * glowPower);
            element.SetPropertyBlock(propBlock);
        }
    }
}