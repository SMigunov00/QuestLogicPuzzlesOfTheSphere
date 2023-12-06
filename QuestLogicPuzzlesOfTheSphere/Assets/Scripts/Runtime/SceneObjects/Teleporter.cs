using Runtime.Save;
using Runtime.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Runtime.SceneObjects
{
    public class Teleporter : SelectableObject, IActivable
    {
        [SerializeField] private Teleporter _friend;
        [SerializeField] private Material _teleportMaterial;
        [SerializeField] private Transform _teleportPoint, _positionPoint;
        [SerializeField] private float _lenghtRay;
        [SerializeField] private LayerMask _teleportedObjectMask;

        public bool IsActive { get; private set; }

        private float _maxEnergy = 250f;
        private Color _color = Color.white;
        [Inject] private LevelSetup _levelSetup;

        private void OnEnable()
        {
            Tweaker.OnChangeGlowPower += HandleChangeGlowPower;
        }

        private void OnDisable()
        {
            Tweaker.OnChangeGlowPower -= HandleChangeGlowPower;
        }

        private void Start()
        {
            GameSettingsData settingsData = JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString("SavedGameSettingsData"));
            _maxEnergy = settingsData.MaxGlowPower;
            _color = _levelSetup.LevelColor;
            UpdateGlow();
        }

        public override void StopAtSelectedPoint(Player player)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.up);
            bool intersect = Physics.Raycast(ray, out hit, _lenghtRay, _teleportedObjectMask);
            if (intersect)
            {
                StartCoroutine(Teleported(hit.collider.gameObject));
                IsActive = true;
            }
        }

        public override Vector3 GetPointPosition() => _positionPoint.position;
        public SelectableObject GetFriend() => _friend.gameObject.GetComponent<SelectableObject>();
        public override List<Lane> GetDirections() => _availableDestinations;
        public override int GetCountFried() => _availableDestinations.Count;

        private Vector3 GetTeleportPoint() => _teleportPoint != null ? _teleportPoint.position : transform.position;

        public void ActivateObject()
        {
            //StartCoroutine(Teleported());
        }

        private IEnumerator Teleported(GameObject hitGameObject)
        {
            Renderer[] elements = hitGameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer element in elements)
            {
                yield return StartCoroutine(UdpateTeleportedEffect(element, 1));
            }
            hitGameObject.transform.position = _friend.GetTeleportPoint();
            foreach (Renderer element in elements)
            {
                yield return StartCoroutine(UdpateTeleportedEffect(element, -1));
            }
        }

        private IEnumerator UdpateTeleportedEffect(Renderer element, int m)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            element.GetPropertyBlock(propBlock);
            Material defaultObjectMaterial = element.material;
            element.material = _teleportMaterial;
            float currentStateEffect = propBlock.GetFloat("_Treshold");
            switch (currentStateEffect)
            {
                case 0f:
                    while (currentStateEffect < 1)
                    {
                        currentStateEffect += Time.deltaTime * m;
                        propBlock.SetFloat("_Treshold", currentStateEffect);
                        element.SetPropertyBlock(propBlock);
                        yield return null;
                    }
                    propBlock.SetFloat("_Treshold", 1f);
                    element.SetPropertyBlock(propBlock);
                    break;
                case 1f:
                    while (currentStateEffect > 0)
                    {
                        currentStateEffect += Time.deltaTime * m;
                        propBlock.SetFloat("_Treshold", currentStateEffect);
                        element.SetPropertyBlock(propBlock);
                        yield return null;
                    }
                    propBlock.SetFloat("_Treshold", 0f);
                    element.SetPropertyBlock(propBlock);
                    break;
            }
            element.material = defaultObjectMaterial;
        }


        public void HandleChangeGlowPower(float newEnergy)
        {
            _maxEnergy = newEnergy;
            UpdateGlow();
        }

        private void UpdateGlow()
        {
            UpdateGlowPower();
            UpdateEmissionColor();
        }

        private float _glowPower;

        private void UpdateGlowPower()
        {
            float glowPower = _maxEnergy / 100.0f;

            float k = 1.442695f;
            _glowPower = Mathf.Log(glowPower + 1) * k;
        }
        private void UpdateEmissionColor()
        {
            UpdateGlowPower();

            Renderer[] elements = GetComponentsInChildren<Renderer>();
            foreach (Renderer element in elements)
            {
                UpdateEmissionColor(element);
            }
        }

        private void UpdateEmissionColor(Renderer element)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            element.GetPropertyBlock(propBlock);
            propBlock.SetColor("_ColorOne", _color * _glowPower);
            propBlock.SetColor("_ColorTwo", _color);
            propBlock.SetColor("_EmissionColor", _color * _glowPower);
            element.SetPropertyBlock(propBlock);
        }
    }
}