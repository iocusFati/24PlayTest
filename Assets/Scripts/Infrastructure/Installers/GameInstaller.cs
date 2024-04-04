using Gameplay.Level;
using Zenject;

namespace Infrastructure
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindLevelGenerator();
        }

        private void BindLevelGenerator()
        {
            Container
                .BindInterfacesAndSelfTo<LevelGenerator>()
                .AsSingle();
        }
    }
}