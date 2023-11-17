using Runtime.Services;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Runtime.SceneObjects
{
    public class PlayerMover : MonoBehaviour
    {
        public event Action OnCompletedStep;

        public Player Player { get; private set; }

        public Coroutine Coroutine { get; private set; }

        IPauseHandler _pauseService;

        [Inject]
        private void Constructor(Player player, IPauseHandler pauseService)
        {
            Player = player;
            _pauseService = pauseService;
        }

        public void StartMovement(SelectableObject selectableObject)
        {
            OnCompletedStep?.Invoke();
            Coroutine = StartCoroutine(Move(selectableObject));
        }

        public void StopMoving()
        {
            if (Coroutine != null)
                StopCoroutine(Coroutine);
            Coroutine = null;
        }

        private IEnumerator Move(SelectableObject selectableObject)
        {

            Vector3 targetPosition = selectableObject.GetPointPosition();
            yield return StartCoroutine(Rotate(targetPosition));
            while (Player.transform.position != targetPosition)
            {
                if (!_pauseService.IsPaused)
                {
                    Player.transform.position = Vector3.MoveTowards(Player.transform.position, targetPosition, Player.MoveSpeed * Time.deltaTime);
                    Player.transform.Rotate(new Vector3(Player.RotateSpeed, 0, 0));
                }
                yield return null;
            }
            Coroutine = null;
            selectableObject.StopAtSelectedPoint(Player);
        }

        private IEnumerator Rotate(Vector3 targetPosition)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - Player.transform.position, Vector3.up);
            while (Quaternion.Angle(Player.transform.rotation, targetRotation) > 0)
            {
                Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, targetRotation, Player.MoveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}