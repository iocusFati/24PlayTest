using Cysharp.Threading.Tasks;
using Gameplay.Level;
using Infrastructure.Services.Input;
using Infrastructure.Services.Pool;
using UI.Mediator;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Infrastructure.States
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IPlayerFactory _playerFactory;
        private readonly SceneLoader _sceneLoader;
        private readonly LevelGenerator _levelGenerator;
        private readonly IPoolService _poolService;
        private readonly IInputService _inputService;
        private readonly IUIMediator _uiMediator;

        private Transform _initialPoint;

        public LoadLevelState(IGameStateMachine gameStateMachine,
            IPlayerFactory playerFactory,
            SceneLoader sceneLoader,
            LevelGenerator levelGenerator, 
            IPoolService poolService, 
            IInputService inputService, 
            IUIMediator uiMediator)
        {
            _gameStateMachine = gameStateMachine;
            _playerFactory = playerFactory;
            _sceneLoader = sceneLoader;
            _levelGenerator = levelGenerator;
            _poolService = poolService;
            _inputService = inputService;
            _uiMediator = uiMediator;
        }

        public void Enter(string sceneName)
        {
            if (sceneName != SceneManager.GetActiveScene().name) 
                _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit()
        {
            
        }

        private void OnLoaded()
        {
            _uiMediator.HideCurtain();
            
            _poolService.SpawnParents();
            _levelGenerator.Initialize();
            _uiMediator.InitializeOnGameSceneLoaded();

            _uiMediator.ShowStartWindow();
            
            _initialPoint = GameObject.FindWithTag(Tags.PlayerSpawn).transform;
            
            Player player = _playerFactory.CreatePlayer(_initialPoint);
            player.Initialize();
            
            EnterGameLoopStateAsync().Forget();
        }
        
        private async UniTaskVoid EnterGameLoopStateAsync()
        {
            await UniTask.WaitUntil(() => _inputService.CanStartMoving());
            
            _gameStateMachine.Enter<GameLoopState>();
            _uiMediator.HideStartWindow();
        }
    }
}