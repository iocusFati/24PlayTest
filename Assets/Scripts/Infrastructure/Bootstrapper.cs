using System;
using Infrastructure.States;
using UI.Mediator;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class Bootstrapper : MonoBehaviour
    {
        private const string GameSceneName = "GameScene";
        
        private IGameStateMachine _gameStateMachine;
        private StatesFactory _statesFactory;
        private IUIMediator _uiMediator;

        [Inject]
        public void Construct(IGameStateMachine gameStateMachine, StatesFactory statesFactory, IUIMediator uiMediator)
        {
            _gameStateMachine = gameStateMachine;
            _statesFactory = statesFactory;
            _uiMediator = uiMediator;
        }

        private void Awake()
        {
            _uiMediator.BootstrapInitialize();
            _uiMediator.ShowCurtain();
            
            RegisterStates();

            _gameStateMachine.Enter<LoadLevelState, string>(GameSceneName);
        }

        private void RegisterStates()
        {
            _gameStateMachine.RegisterState(_statesFactory.Create<LoadLevelState>());
            _gameStateMachine.RegisterState(_statesFactory.Create<ReloadLevelState>());
            _gameStateMachine.RegisterState(_statesFactory.Create<GameLoopState>());
            _gameStateMachine.RegisterState(_statesFactory.Create<GameLostState>());
        }
    }
}