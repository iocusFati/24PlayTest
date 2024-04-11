using Base.UI.Factory;
using Gameplay.Level;
using Infrastructure.AssetProviderService;
using Infrastructure.Services.Input;
using Infrastructure.Services.Pool;
using Infrastructure.Services.StaticDataService;
using Infrastructure.States;
using UI.Mediator;
using UnityEngine;
using Zenject;
using IAssets = Infrastructure.AssetProviderService.IAssets;

namespace Infrastructure
{
    public class BootstrapInstaller : MonoInstaller, ICoroutineRunner
    {
        [SerializeField] private Updater _updater;

        public override void InstallBindings()
        {
            BindStaticData();
            BindSceneLoader();
            BindStatesFactory();
            BindCoroutineRunner();
            BindGameStateMachine();
            BindAssetsService();
            BindPoolService();
            BindLevelGenerator();
            BindInputService();
            BindUIMediator();
            BindFactories();
            BindUpdater();
        }

        private void BindUIMediator()
        {
            Container
                .Bind<IUIMediator>()
                .To<UIMediator>()
                .AsSingle();
        }

        private void BindUpdater()
        {
            Container
                .Bind<IUpdater>()
                .FromInstance(_updater)
                .AsSingle();
        }

        private void BindInputService() =>
            Container
                .Bind<IInputService>()
                .FromMethod(InputService)
                .AsSingle();

        private void BindLevelGenerator()
        {
            Container
                .BindInterfacesAndSelfTo<LevelGenerator>()
                .AsSingle();
        }

        private void BindStaticData()
        {
            StaticDataService staticData = Container.Instantiate<StaticDataService>();

            Container
                .Bind<IStaticDataService>()
                .FromInstance(staticData)
                .AsSingle();
            
            staticData.Initialize();
        }

        private void BindPoolService()
        {
            PoolService poolService = Container.Instantiate<PoolService>();
            
            Container
                .Bind<IPoolService>()
                .FromInstance(poolService)
                .AsSingle();
        }

        private void BindAssetsService()
        {
            Container
                .Bind<IAssets>()
                .To<AssetProvider>()
                .AsSingle();
        }
        
        private void BindFactories()
        {
            BindPlayerFactory();
            BindUIFactory();

            void BindPlayerFactory()
            {
                Container
                    .Bind<IPlayerFactory>()
                    .To<PlayerFactory>()
                    .AsSingle();
            }
            
            void BindUIFactory()
            {
                Container
                    .Bind<IUIFactory>()
                    .To<UIFactory>()
                    .AsSingle();
            }
        }

        private void BindSceneLoader()
        {
            Container
                .Bind<SceneLoader>()
                .AsSingle();
        }

        private void BindStatesFactory()
        {
            Container
                .BindInterfacesAndSelfTo<StatesFactory>()
                .AsSingle();
        }

        private void BindCoroutineRunner()
        {
            Container
                .Bind<ICoroutineRunner>()
                .FromInstance(this)
                .AsSingle();
        }

        private void BindGameStateMachine()
        {
            Container
                .Bind<IGameStateMachine>()
                .To<GameStateMachine>()
                .FromInstance(new GameStateMachine())
                .AsSingle();
        }

        private IInputService InputService() =>
            Application.isEditor
                ? new StandaloneInput()
                : new MobileInput();
    }
}
