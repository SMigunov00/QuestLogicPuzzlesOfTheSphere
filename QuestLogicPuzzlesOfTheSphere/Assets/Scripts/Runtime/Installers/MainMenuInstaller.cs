using Runtime.Save;
using Zenject;

namespace Runtime.Installer
{
    public class MainMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<DataStorageInMainMenu>().AsSingle().NonLazy();
        }
    }
}