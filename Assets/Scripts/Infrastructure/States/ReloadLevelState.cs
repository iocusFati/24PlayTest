using Base.UI.Factory;
using Cysharp.Threading.Tasks;
using Gameplay.Level;
using Infrastructure.Services.Pool;
using UnityEngine;

namespace Infrastructure.States
{
    public class ReloadLevelState : IState
    {
        private readonly IUIFactory _uiFactory;
        private readonly LevelGenerator _levelGenerator;
        private readonly BasePool<Transform> _simpleCubePool;
        
        private ScreenFader _screenFader;
        private Player _player;

        public ReloadLevelState(LevelGenerator levelGenerator, IUIFactory uiFactory, IPlayerFactory playerFactory,
            IPoolService poolService)
        {
            _levelGenerator = levelGenerator;
            _uiFactory = uiFactory;
            _simpleCubePool = poolService.SimpleCubes;
            
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
            
            _player.BaseCubeToInitialPosition();

            await _screenFader.UnfadeAsync();
            
            _player.Initialize();
            _screenFader.Hide();
        }
    }
}