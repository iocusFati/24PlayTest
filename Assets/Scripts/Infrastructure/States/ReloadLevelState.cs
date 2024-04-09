using Base.UI.Factory;
using Cysharp.Threading.Tasks;
using Gameplay.Level;
using Infrastructure.Services.Input;
using Infrastructure.Services.Pool;
using UnityEngine;

namespace Infrastructure.States
{
    public class ReloadLevelState : IState
    {
        private readonly IUIFactory _uiFactory;
        private readonly LevelGenerator _levelGenerator;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IInputService _inputService;
        private readonly BasePool<Transform> _simpleCubePool;

        private ScreenFader _screenFader;
        private Player _player;

        public ReloadLevelState(LevelGenerator levelGenerator, IUIFactory uiFactory, IPlayerFactory playerFactory,
            IPoolService poolService, IGameStateMachine gameStateMachine, IInputService inputService)
        {
            _levelGenerator = levelGenerator;
            _uiFactory = uiFactory;
            _simpleCubePool = poolService.SimpleCubes;
            _gameStateMachine = gameStateMachine;
            _inputService = inputService;
            
            playerFactory.OnPlayerCreated += player => _player = player;
        }


        public void Enter()
        {
            if (_screenFader == null) 
                _screenFader = _uiFactory.CreateScreenFader();

            ReloadLevel().Forget();
        }

        public void Exit()
        {
            
        }

        private async UniTaskVoid ReloadLevel()
        {
            _screenFader.Show();
            await _screenFader.FadeAsync();

            _simpleCubePool.ReleaseAll();
            _levelGenerator.HideAllChunks();
            _levelGenerator.GenerateFirstChunks();

            _player.PauseTrail();
            _player.BaseCubeToInitialState();
            _player.SpawnStickman();

            await _screenFader.UnfadeAsync();
            
            _player.Initialize();
            _screenFader.Hide();
            
            EnterGameLoopStateAsync().Forget();
        }
        
        private async UniTaskVoid EnterGameLoopStateAsync()
        {
            await UniTask.WaitUntil(() => _inputService.CanStartMoving());
            
            _gameStateMachine.Enter<GameLoopState>();
        }
    }
}