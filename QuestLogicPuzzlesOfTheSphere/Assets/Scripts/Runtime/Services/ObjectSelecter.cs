using Runtime.SceneObjects;
using System;
using UnityEngine;
using Zenject;

namespace Runtime.Services
{
    public class ObjectSelecter : IInitializable, IDisposable
    {
        private LayerMask _selectableObjectMask;

        private SelectableObject _lastSelectableObject;

        private RaycastHit _hitInfo;

        private IInput _inputService;

        private PlayerMover _playerMover;

        private Lane _lane;
        private IPauseHandler _pauseHandler;

        [Inject]
        private void Constructor(LevelSetup levelSetup, IInput inputService, PlayerMover playerMover, SelectableObject startPoint, IPauseHandler pauseHandler)
        {
            _inputService = inputService;
            _playerMover = playerMover;
            _lastSelectableObject = startPoint;
            _selectableObjectMask = levelSetup.SelectableObjectMask;
            _pauseHandler = pauseHandler;
        }

        private void OnClick(Vector3 position) =>
          SelectObject(position);

        private void SelectObject(Vector3 position)
        {
            if (!_pauseHandler.IsPaused)
            {
                Ray ray = Camera.main.ScreenPointToRay(position);
                if (Physics.Raycast(ray, out _hitInfo, Mathf.Infinity, _selectableObjectMask))
                {
                    SelectableObject selectableObject = _hitInfo.collider.gameObject.GetComponent<SelectableObject>();
                    if (_playerMover.Coroutine == null && IsSelectableFriend(selectableObject) && _lastSelectableObject != selectableObject)
                    {
                        if (!_lastSelectableObject.CheckLaneActivated(selectableObject.gameObject.transform.position, _playerMover.Player))
                        {
                            _playerMover.StartMovement(selectableObject);
                            if (selectableObject.gameObject.GetComponent<Teleporter>())
                                _lastSelectableObject = selectableObject.gameObject.GetComponent<Teleporter>().GetFriend();
                            else
                                _lastSelectableObject = selectableObject;
                        }
                    }
                }
            }
        }

        private bool IsSelectableFriend(SelectableObject selectableObject)
        {

            var lastSelectableObjectDirections = _lastSelectableObject.GetDirections();
            var CurrentSelectableObjectDirections = selectableObject.GetDirections();
            for (int x = 0; x < lastSelectableObjectDirections.Count; x++)
            {
                for (int y = 0; y < CurrentSelectableObjectDirections.Count; y++)
                {
                    if (lastSelectableObjectDirections[x] == CurrentSelectableObjectDirections[y])
                    {
                        _lane = CurrentSelectableObjectDirections[y];
                        return true;
                    }
                }
            }
            return false;
        }

        public void Initialize()
        {
            _inputService.OnClick += OnClick;
        }

        public void Dispose()
        {
            _inputService.OnClick -= OnClick;
        }
    }
}