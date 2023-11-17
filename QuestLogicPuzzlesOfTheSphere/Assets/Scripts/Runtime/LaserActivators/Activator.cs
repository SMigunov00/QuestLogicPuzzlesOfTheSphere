using UnityEngine;

namespace Runtime.LaserActivators
{
    public abstract class Activator : MonoBehaviour
    {
        [Range(0, 1000)]
        [SerializeField] private float _energy = 100;
        [SerializeField] private Color _color = Color.white;

        public Color GetColor() => _color;
        public float GetEnergy() => _energy;

        private void OnValidate()
        {
            UpdateEmissionColor();
        }

        public void SetColor(Color color)
        {
            _color = color;
            UpdateEmissionColor();
        }

        public void SetEnergy(float value)
        {
            _energy = value;
            UpdateEmissionColor();
        }

        private void UpdateEmissionColor()
        {
            float glowPower = _energy / 100.0f;

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
            propBlock.SetColor("_EmissionColor", _color * glowPower);
            element.SetPropertyBlock(propBlock);
        }
    }
}