using Runtime.SceneObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ActivableObjectActivator : ITickable
{
    private float _lenghtRay = 3f;

    private List<IActivable> _activableObjectList = new List<IActivable>();

    private IActivable _lastActivableObject;

    private Player _player;
    private PlayerMover _playerMover;

    [Inject]
    private void Costructor(Player player, PlayerMover playerMover)
    {
        _player = player;
        _playerMover = playerMover;
    }

    private List<Lane> _lanes = new List<Lane>();
    private List<ActivablePoint> _activablePoins = new List<ActivablePoint>();

    public void Tick()
    {
        RaycastHit hit;
        Ray ray = new Ray(_player.gameObject.transform.position, Vector3.down);
        bool intersect = Physics.Raycast(ray, out hit, _lenghtRay);
        if (intersect && _playerMover.Coroutine != null)
        {
            IActivable activableObject = hit.collider.gameObject.GetComponent<IActivable>();
            if (activableObject != null && !activableObject.IsActive && _lastActivableObject != activableObject)
            {
                GameObject gameObject = hit.collider.gameObject;
                if (gameObject.GetComponent<Lane>())
                    _lanes.Add(gameObject.GetComponent<Lane>());
                else if (gameObject.GetComponent<ActivablePoint>())
                    _activablePoins.Add(gameObject.GetComponent<ActivablePoint>());

                _lastActivableObject = activableObject;
                _activableObjectList.Add(_lastActivableObject);
                activableObject.ActivateObject();
            }
        }
        else if (_playerMover.Coroutine == null && _activablePoins.Count > 0)
        {
            
           /*foreach (var point in _activablePoins)
            {
                foreach (Lane lane in _lanes)
                {
                    //point.RemoveSelectedObjectAtList(lane);
                }
            }*/
           if (_lanes != null)
               _lanes.Clear();
           /* if (isDeleted)
            {
                _lanes.Clear();
            }*/
        }
    }
}
