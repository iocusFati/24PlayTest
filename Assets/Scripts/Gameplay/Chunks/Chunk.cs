using System;
using System.Collections.Generic;
using Infrastructure.States;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace Gameplay.Level
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private Transform _end;
        [SerializeField] private ParticleSystem _warp;
        [SerializeField] private List<GameObject> _pickups;

        private UniqueId _uniqueId;

        private string _id;
        private IGameStateMachine _gameStateMachine;
        
        private CompositeDisposable _stateMachineDisposer;

        [Inject]
        public void Construct(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }
        
        public string Id
        {
            get
            {
                if (_id is null or "")
                {
                    if (_uniqueId is null)
                    {
                        _uniqueId = GetComponent<UniqueId>();
                    }
                    
                    if (_uniqueId is null)
                    {
                        Debug.Log(this);
                        return null;
                    }

                    Id = _uniqueId.Id;
                }

                return _id;
            }
            private set => 
                _id = value;
        }

        public Vector3 End => _end.position;

        private void Awake()
        {
            _stateMachineDisposer = new CompositeDisposable();
            
            _uniqueId = GetComponent<UniqueId>();

            if (_uniqueId is null)
                return;
            
            Id = _uniqueId.Id;
        }

        private void OnEnable()
        {
            ActivatePickups();
            TryPlayWarp();
        }

        private void OnDisable()
        {
            _stateMachineDisposer.Clear();
            _warp.Stop();
        }

        private void ActivatePickups()
        {
            foreach (var pickup in _pickups) 
                pickup.gameObject.SetActive(true);
        }

        private void TryPlayWarp()
        {
            if (_gameStateMachine.CompareCurrentState<GameLoopState>())
            {
                _warp.Play();
                SubscribeForWarpStop();
            }
            else
            {
                SubscribeForWarpPlay();
            }
        }

        private void SubscribeForWarpStop() =>
            _gameStateMachine.CurrentState
                .Where(_ => !_gameStateMachine.CompareCurrentState<GameLoopState>())
                .Subscribe(_ => _warp.Stop())
                .AddTo(_stateMachineDisposer);

        private void SubscribeForWarpPlay() =>
            _gameStateMachine.CurrentState
                .Where(_ => _gameStateMachine.CompareCurrentState<GameLoopState>())
                .Subscribe(_ =>
                {
                    _warp.Play();
                    _stateMachineDisposer.Clear();
                    SubscribeForWarpStop();
                })
                .AddTo(_stateMachineDisposer);
    }
}