﻿using Cinemachine;
using Cysharp.Threading.Tasks;
using Gameplay.Camera;
using Gameplay.Level;
using Infrastructure.Services.Input;
using Infrastructure.Services.Pool;
using Infrastructure.Services.StaticDataService;
using Infrastructure.StaticData.PlayerData;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;
using Zenject;

namespace Infrastructure.States
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Rigidbody _baseCubeRB;
        
        private IInputService _inputService;

        private LevelGenerator _levelGenerator;
        private PlayerConfig _playerConfig;
        private IPoolService _poolService;
        private IGameStateMachine _gameStateMachine;

        private PlayerMovement _playerMovement;
        private PlayerStack _playerStack;

        [Inject]
        public void Construct(LevelGenerator levelGenerator, IStaticDataService staticData, IInputService inputService,
            IPoolService poolService, IGameStateMachine gameStateMachine, IUpdater updater)
        {
            _levelGenerator = levelGenerator;
            _playerConfig = staticData.PlayerConfig;
            _inputService = inputService;
            _poolService = poolService;
            _gameStateMachine = gameStateMachine;

            _playerMovement = new PlayerMovement(inputService, _playerConfig, transform, _baseCubeRB, updater);
        }

        private void Awake()
        {
            InitializePlayerStack();
            
            SubscribeForBaseCubeEvents();
            
            // CinemachineCore.Instance.GetActiveBrain(0)
            //     .ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CameraMovement>()
            //     .SetTarget(_baseCubeRB.transform);
            CinemachineCore.Instance.GetActiveBrain(0)
                .ActiveVirtualCamera.Follow = _baseCubeRB.transform;
        }


        private void PickUp(Collider other)
        {
            other.gameObject.SetActive(false);
            _playerStack.AddCube();
        }

        private void InitializePlayerStack()
        {
            _playerStack = GetComponent<PlayerStack>();
            _playerStack.Construct(_playerConfig, _poolService);
            _playerStack.OnLost += Lose;
        }

        private void Lose()
        {
            _playerMovement.StopMoving();
            
            _gameStateMachine.Enter<GameLostState>();
        }


        private void SubscribeForBaseCubeEvents()
        {
            _baseCubeRB.OnTriggerEnterAsObservable()
                .Where(other => other.CompareTag(Tags.Pickup))
                .Subscribe(PickUp);
            _baseCubeRB.OnTriggerExitAsObservable()
                .Where(other => other.CompareTag(Tags.WallTrigger))
                .Subscribe(_ => _levelGenerator.SpawnNextChunk());
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tags.Wall))
            {
                _levelGenerator.SpawnNextChunk();
            }
        }

        public void Initialize()
        {
            StartMovementOnGetMovement().Forget();
            
            // CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.Follow = _baseCubeRB.transform;
        }

        public void BaseCubeToInitialPosition()
        {
            _playerStack.Initialize();

            _playerStack.HolderToInitialPosition();
            _baseCubeRB.transform.localPosition = Vector3.zero;
        }

        private async UniTaskVoid StartMovementOnGetMovement()
        {
            await UniTask.WaitUntil(() => _inputService.GetMovement() != 0);
            
            _playerMovement.StartMoving();
        }
    }
}