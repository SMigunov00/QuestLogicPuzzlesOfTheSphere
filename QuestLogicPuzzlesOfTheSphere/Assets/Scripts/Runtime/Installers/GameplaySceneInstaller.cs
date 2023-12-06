using Runtime.SceneObjects;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Runtime.Installer
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private SelectableObject _startPoint;

        public override void InstallBindings()
        {
            InitializeGameplaySceneObject();
            BindPlayer();
        }

        private void InitializeGameplaySceneObject()
        {
            var activablePoints = FindObjectsOfType<ActivablePoint>();
            foreach (var point in activablePoints)
            {
                if (point.IsStartPoint)
                    _startPoint = point.GameObject().gameObject.GetComponent<ActivablePoint>();
            }
        }

        private void BindPlayer()
        {
            Player player = Container.InstantiatePrefabForComponent<Player>(_playerPrefab, _startPoint.GetPointPosition(), Quaternion.identity, null);
            Container.BindInstance(player).AsSingle().NonLazy();
            Container.BindInstance(player.gameObject.GetComponent<ItemPicker>()).AsSingle().NonLazy();
            Container.BindInstance(_startPoint).AsSingle().NonLazy();
        }
    }
}