using Gameplay.Level;
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

        private Transform _initialPoint;

        public LoadLevelState(IGameStateMachine gameStateMachine,
            ISaveLoadService saveLoadService,
            IPlayerFactory playerFactory,
            SceneLoader sceneLoader, 
            LevelGenerator levelGenerator)
        {
            _gameStateMachine = gameStateMachine;
            _saveLoadService = saveLoadService;
            _playerFactory = playerFactory;
            _sceneLoader = sceneLoader;
            _levelGenerator = levelGenerator;
        }

        public void Enter(string sceneName)
        {
            if (sceneName != SceneManager.GetActiveScene().name)
            {
                _sceneLoader.Load(sceneName, OnLoaded);
            }
            else
            {
                Reload();
            }
        }

        public void Exit()
        {
            
        }

        private void OnLoaded()
        {
            _levelGenerator.Initialize();
            
            _initialPoint = GameObject.FindWithTag(Tags.PlayerSpawn).transform;
            
            Player player = _playerFactory.CreatePlayer(_initialPoint);
            player.Initialize();
            
            //
            // _saveLoadService.InformReaders();
            // _gameStateMachine.Enter<GameLoopState>();
        }

        private void Reload()
        {
        }
    }
}