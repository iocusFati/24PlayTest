using Gameplay.Level;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Infrastructure
{
    public class GameInstaller : MonoInstaller
    {
        public CinemachineImpulseInvoker impulseInvoker;

        public override void InstallBindings()
        {
            BindImpulseInvoker();
        }

        private void BindImpulseInvoker()
        {
            Container.ParentContainers[0].Bind<CinemachineImpulseInvoker>()
                .FromInstance(impulseInvoker)
                .AsSingle();
        }
    }
}