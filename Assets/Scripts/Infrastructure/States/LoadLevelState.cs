using Cysharp.Threading.Tasks;
using Gameplay.Level;
using Infrastructure.Services.Input;
using Infrastructure.Services.Pool;
using Infrastructure.Services.SaveLoad;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Infrastructure.States
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IPlayerFactory _playerFactory;
        private readonly SceneLoader _sceneLoader;
        private readonly LevelGenerator _levelGenerator;
        private readonly IPoolService _poolService;
        private IInputService _inputService;

        private Transform _initialPoint;

        public LoadLevelState(IGameStateMachine gameStateMachine,
            ISaveLoadService saveLoadService,
            IPlayerFactory playerFactory,
            SceneLoader sceneLoader,
            LevelGenerator levelGenerator, 
            IPoolService poolService, 
            IInputService inputService)
        {
            _gameStateMachine = gameStateMachine;
            _saveLoadService = saveLoadService;
            _playerFactory = playerFactory;
            _sceneLoader = sceneLoader;
            _levelGenerator = levelGenerator;
            _poolService = poolService;
            _inputService = inputService;
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
            Debug.Log("OnLoaded");
            _poolService.SpawnParents();
            _levelGenerator.Initialize();
            
            _initialPoint = GameObject.FindWithTag(Tags.PlayerSpawn).transform;
            
            Player player = _playerFactory.CreatePlayer(_initialPoint);
            player.Initialize();
            
            EnterGameLoopStateAsync().Forget();
        }
        
        private async UniTaskVoid EnterGameLoopStateAsync()
        {
            await UniTask.WaitUntil(() => _inputService.CanStartMoving());
            
            _gameStateMachine.Enter<GameLoopState>();
        }
    }
}