using System;
using System.Collections;
using Base.UI.Factory;
using Gameplay.Level;
using Infrastructure.AssetProviderService;
using Infrastructure.Services.Input;
using Infrastructure.Services.PersistentProgress;
using Infrastructure.Services.Pool;
using Infrastructure.Services.SaveLoad;
using Infrastructure.Services.StaticDataService;
using Infrastructure.States;
using UnityEngine;
using Zenject;

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
            BindSaveLoadService();
            BindPersistentProgress();
            BindAssetsService();
            BindPoolService();
            BindLevelGenerator();
            BindInputService();

            BindFactories();

            BindUpdater();
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

        private void BindPersistentProgress()
        {
            Container
                .Bind<IPersistentProgressService>()
                .To<PersistentProgressService>()
                .AsSingle();
        }

        private void BindSaveLoadService()
        {
            Container
                .Bind<ISaveLoadService>()
                .To<SaveLoadService>()
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

        public void DoAfter(Func<bool> condition, Action action) => 
            StartCoroutine(DoAfterCoroutine(condition, action));

        private IEnumerator DoAfterCoroutine(Func<bool> condition, Action action)
        {
            yield return new WaitUntil(condition);

            Debug.Log("Action");
            action.Invoke();
        }

        private IInputService InputService() =>
            Application.isEditor
                ? new StandaloneInput()
                : new MobileInput();
    }
}
