using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace Runtime.SceneObjects.Training
{
    public class Educator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _learningUIText;
        [SerializeField] private float _lenghtRay;
        [SerializeField] private LayerMask _teleportedObjectMask;
        [SerializeField] private string _learningText;

        private bool _isLearnedShow;

        private IPauseHandler _pause;

        [SerializeField]private Player _player;

        [Inject]
        private void Constructor(IPauseHandler pause)
        {
            _pause = pause;
        }

        private void Update()
        {
            if (!_pause.IsPaused)
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, transform.up);
                bool intersect = Physics.Raycast(ray, out hit, _lenghtRay, _teleportedObjectMask);
                if (intersect)
                {
                    if (!_isLearnedShow)
                    {
                        _isLearnedShow = true;
                        _learningUIText.text = _learningText;
                        _player = hit.collider.gameObject.GetComponent<Player>();
                    }
                }
                else
                {
                    if (_player != null)
                    {
                        _learningUIText.text = null;
                        _player = null;
                    }
                }
            }
        }
    }
}