using Runtime.Interfaces;
using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(LineRenderer))]
    public class Laser : MonoBehaviour
    {
        private const float MaxLength = 100.0f;
        [SerializeField] private float _energy = 100;
        private LineRenderer _lineRenderer;
        private IDamagable _lastDamagableObject;
        private Vector3 _laserStartPosition;
        private Vector3 _laserEndPosition;
        private Vector3 _hitPoint;
        private Vector3 _hitNormal;
        private Color _color;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public Color GetColor() => _color;

        public float GetEnergy() => _energy;

        public void SetEnergy(float value)
        {
            _energy = value;
            UpdateGlowLevel();
        }

        public void SetColor(Color color)
        {
            _color = color;
            UpdateGlowLevel();
        }

        public void SetParameters(Color color, float energy)
        {
            _color = color;
            _energy = energy;
            UpdateGlowLevel();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void UpdateGlowLevel()
        {
            float glowPower = _energy / 100.0f;

            float k = 1.8f;
            glowPower = Mathf.Log(1.5f + 1) * k;


            Renderer element = GetComponentInChildren<Renderer>();

            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            element.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", _color * glowPower);
            element.SetPropertyBlock(propBlock);
        }

        public Vector3 GetStartPosition() => _laserStartPosition;
        public Vector3 GetEndPosition() => _laserEndPosition;
        public Vector3 GetHitPoint() => _hitPoint;
        public Vector3 GetHitNormal() => _hitNormal;


        public void Process()
        {

            _laserStartPosition = transform.position;
            _laserEndPosition = transform.position + transform.forward * _energy;

            // Чтобы небыло пересечений с самим собой при движении.
            RaycastHit hit;
            Ray ray = new Ray(_laserStartPosition, transform.forward);
            bool intersect = Physics.Raycast(ray, out hit, _energy);

            if (intersect)
            {
                IDamagable damagableObject = hit.collider.gameObject.GetComponentInParent<IDamagable>();

                _hitPoint = hit.point;
                _laserEndPosition = hit.point;
                _hitNormal = hit.normal;

                if (_lastDamagableObject == null)
                {
                    //     HandleLaserReceiver(damagableObject);
                    damagableObject.SetDamage();
                    _lastDamagableObject = damagableObject;
                }
                /*else
                {
                    HandleNoLaserReceiver();
                }*/
            }
            else
            {
                HandleNoLaserReceiver();
            }

            UpdateLine();
        }

        /*private void HandleLaserReceiver(IDamagable damagableObject)
        {
            if (damagableObject != _lastDamagableObject)
            {
                _lastDamagableObject?.OnLaserExit(this);
                damagableObject.OnLaserEnter(this);
            }
            else
            {
                damagableObject.OnLaserMove(this);
            }

            _lastDamagableObject = damagableObject;
        }
        */
        private void HandleNoLaserReceiver()
        {
            //_lastDamagableObject?.OnLaserExit(this);
            _lastDamagableObject = null;
        }

        private void UpdateLine()
        {
            // Наш луч испускается из глобальных координат относительно точки выстрела, поэтому для корректного
            // отображения lineRenderer - трансформируем глобальные координаты в локальные относительно своего transform.

            Vector3 localStartPosition = transform.InverseTransformPoint(_laserStartPosition);
            Vector3 localEndPosition = transform.InverseTransformPoint(_laserEndPosition);

            _lineRenderer.SetPosition(0, localStartPosition);
            _lineRenderer.SetPosition(1, localEndPosition);
        }

        /*public void Stop()
        {
            HandleNoLaserReceiver();
            gameObject.SetActive(false);
        }*/
    }
}