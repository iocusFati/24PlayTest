using Base.UI.Factory;
using Infrastructure.Services.Input;

namespace Infrastructure.States
{
    public class GameLostState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IInputService _inputService;
        private readonly IUIFactory _uiFactory;

        private readonly float _timeBeforeLose;
        private readonly float _cameraRotateDuration;
        
        private LostPopUp _lostPopUp;

        public GameLostState(IGameStateMachine gameStateMachine, IUIFactory uiFactory)
        {
            _gameStateMachine = gameStateMachine;
            _uiFactory = uiFactory;
        }
        
        public void Enter()
        {
            if (_lostPopUp is null)
            {
                _lostPopUp = _uiFactory.CreateLostPopUp();
                _lostPopUp.OnReplayButtonClicked += RestartGame;
            }

            _lostPopUp.Show();
        }

        public void Exit()
        {
        }
        
        private void RestartGame()
        {
            _gameStateMachine.Enter<ReloadLevelState>();
            _lostPopUp.Hide();
        }
    }
}