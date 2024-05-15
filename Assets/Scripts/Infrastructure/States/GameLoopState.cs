using Infrastructure.Services.Analytics;

namespace Infrastructure.States
{
    public class GameLoopState : IState
    {
        private readonly IAnalyticsService _analyticsService;

        public GameLoopState(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }
        
        public void Enter()
        {
            _analyticsService.SendEvent(AnalyticsEvents.GameStarted);
        }
        
        public void Exit()
        {
            
        }
    }
}