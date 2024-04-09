using System;
using System.Collections.Generic;
using UniRx;

namespace Infrastructure.States
{
    public class GameStateMachine : IGameStateMachine
    {
        private readonly Dictionary<Type, IExitState> _states = new();
        public ReactiveProperty<IExitState> CurrentState { get; } = new();

        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            TState state = ChangeState<TState>();
            state.Enter(payload);
        }
        
        public void RegisterState<TState>(TState state) where TState : IExitState =>
            _states.Add(typeof(TState), state);

        public bool CompareCurrentState<TState>() where TState : IExitState => 
            CurrentState.Value.GetType() == typeof(TState);

        private TState ChangeState<TState>() where TState : class, IExitState
        {
            CurrentState.Value?.Exit();

            TState state = GetState<TState>();
            CurrentState.Value = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitState => 
            _states[typeof(TState)] as TState;
    }
}