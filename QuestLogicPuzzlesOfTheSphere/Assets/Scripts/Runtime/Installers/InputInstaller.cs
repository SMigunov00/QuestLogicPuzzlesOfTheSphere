using Runtime.Services;
using UnityEngine;
using Zenject;

namespace Runtime.Installer
{
    public class InputInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            if (SystemInfo.deviceType == DeviceType.Handheld)
                Container.BindInterfacesAndSelfTo<MobileInput>().AsSingle();
            else
                Container.BindInterfacesAndSelfTo<MouseInput>().AsSingle();
        }
    }
}