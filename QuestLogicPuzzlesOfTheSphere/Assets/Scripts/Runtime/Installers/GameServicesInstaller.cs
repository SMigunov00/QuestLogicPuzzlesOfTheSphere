using Runtime.Save;
using Runtime.SceneObjects;
using Runtime.Services;
using UnityEngine;
using Zenject;

namespace Runtime.Installer
{
    public class GameServicesInstaller : MonoInstaller
    {
        [SerializeField] private PlayerMover _playerMover;
        [SerializeField] private LevelSetup _levelSetup;

        public override void InstallBindings()
        {
            BindGameManagers();
        }

        private void BindGameManagers()
        {
            Container.BindInstance(_playerMover).AsSingle().NonLazy();
            Container.BindInstance(_levelSetup).AsSingle().NonLazy();
            Container.Bind<GameState>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PauseSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Storage>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ActivatedPointCounter>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<StepReader>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Stargazer>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Timer>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ObjectSelecter>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ActivableObjectActivator>().AsSingle().NonLazy();
        }
    }
}