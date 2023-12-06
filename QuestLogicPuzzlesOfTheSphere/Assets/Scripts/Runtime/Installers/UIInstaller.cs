using Runtime.SceneObjects;
using Runtime.Services;
using UnityEngine;
using Zenject;

namespace Runtime.Installer
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private VictoryMenu _victoryMenu;
        [SerializeField] private LoseMenu _loseMenu;
        [SerializeField] private HUD _hud;

        public override void InstallBindings()
        {
            InitializeUIObject();
            BindUIElements();
        }

        private void InitializeUIObject()
        {
            _victoryMenu = FindObjectOfType<VictoryMenu>(true);
            _loseMenu = FindObjectOfType<LoseMenu>(true);
            _hud = FindObjectOfType<HUD>();
        }

        private void BindUIElements()
        {
            Container.BindInstance(_hud).AsSingle().NonLazy();
            Container.BindInstance(_victoryMenu).AsSingle().NonLazy();
            Container.BindInstance(_loseMenu).AsSingle().NonLazy();
        }
    }
}