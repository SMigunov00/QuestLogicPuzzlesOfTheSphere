using Runtime.Services.Pause;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Runtime.SceneObjects
{
    public class ActivableObjectActivator : ITickable
    {
        public static Action<bool> OnObjectActivates;

        private float _lenghtRay = 3f;

        private IActivable _lastActivableObject;

        private Player _player;
        private PlayerMover _playerMover;
        private IPauseHandler _pause;

        private bool _haveObjectActivates = false;

        [Inject]
        private void Costructor(Player player, PlayerMover playerMover, IPauseHandler pause)
        {
            _player = player;
            _playerMover = playerMover;
            _pause = pause;
        }

        public void Tick()
        {
            if (!_pause.IsPaused)
            {
                ActivateActivableObject();
                if (_transparentLaneList != null && _activablePointWithTrasnparentLaneList != null)
                    _haveObjectActivates = CheckActivatesActivableObjectWithTransparentLane();
            }
        }

        private SelectableObject _lastSelectableObject;

        private void ActivateActivableObject()
        {
            RaycastHit hit;
            Ray ray = new Ray(_player.gameObject.transform.position, Vector3.down);
            bool intersect = Physics.Raycast(ray, out hit, _lenghtRay);
            if (intersect && _playerMover.Coroutine != null)
            {
                IActivable activableObject = hit.collider.GetComponent<IActivable>();
                if (activableObject != null && _lastActivableObject != activableObject)
                {
                    _lastSelectableObject = hit.collider.GetComponent<SelectableObject>();
                    if (!activableObject.IsActive)
                    {
                        _lastActivableObject = activableObject;
                        activableObject.ActivateObject();
                    }
                }
            }
            if (_playerMover.Coroutine == null && _lastSelectableObject != null && !_haveObjectActivates)
            {
                _lastSelectableObject?.StopAtSelectedPoint(_player);
                _lastSelectableObject = null;
            }
        }

        private List<ActivablePointWithTransparentPaths> _activablePointWithTrasnparentLaneList = new List<ActivablePointWithTransparentPaths>();
        private List<TransparentLane> _transparentLaneList = new List<TransparentLane>();

        public void AddPointWithTransparentPaths(ActivablePointWithTransparentPaths point) => _activablePointWithTrasnparentLaneList.Add(point);
        public void AddTransparentLane(TransparentLane lane) => _transparentLaneList.Add(lane);

        private bool CheckActivatesActivableObjectWithTransparentLane()
        {
            if (_activablePointWithTrasnparentLaneList.Any(x => x.ObjectActivates) || _transparentLaneList.Any(x => x.ObjectActivates))
                return true;
            else
                return false;
        }
    }
}