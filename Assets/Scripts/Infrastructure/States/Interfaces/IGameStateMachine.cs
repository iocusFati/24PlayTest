using UniRx;

namespace Infrastructure.States
{
    public interface IGameStateMachine
    {
        ReactiveProperty<IExitState> CurrentState { get; }
        
        void Enter<TState>() where TState : class, IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
        void RegisterState<TState>(TState state) where TState : IExitState;
        bool CompareCurrentState<TState>() where TState : IExitState;
    }
}