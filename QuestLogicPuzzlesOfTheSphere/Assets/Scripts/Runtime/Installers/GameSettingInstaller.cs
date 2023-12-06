using Runtime.Localization;
using Unity.VisualScripting;
using Zenject;

namespace Runtime.Installer
{
    public class GameSettingInstaller : MonoInstaller
    {
        private Localizator _localizator;

        public override void InstallBindings()
        {
            InitializeGameSettingObject();
            Container.BindInstance(_localizator).AsSingle().NonLazy();
        }

        private void InitializeGameSettingObject()
        {
            _localizator = FindObjectOfType(typeof(Localizator)).GetComponent<Localizator>();
        }
    }
}