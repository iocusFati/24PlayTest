using System;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
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

        private PlayerMovement _playerMovement;
        private PlayerStack _playerStack;
        private PlayerCollisions _playerCollisions;

        [Inject]
        public void Construct(LevelGenerator levelGenerator, IStaticDataService staticData, IInputService inputService,
            IPoolService poolService)
        {
            _levelGenerator = levelGenerator;
            _playerConfig = staticData.PlayerConfig;
            _inputService = inputService;
            _poolService = poolService;

            _playerMovement = new PlayerMovement(inputService, _playerConfig, transform, _baseCubeRB);
        }

        private void Awake()
        {
            _playerStack = GetComponent<PlayerStack>();
            _playerStack.Construct(_playerConfig, _poolService);

            SubscribeForBaseCubeEvents();
        }


        private void PickUp(Collider other)
        {
            other.gameObject.SetActive(false);
            _playerStack.AddCube();
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

            CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.Follow = _baseCubeRB.transform;
        }

        private async UniTaskVoid StartMovementOnGetMovement()
        {
            await UniTask.WaitUntil(() => _inputService.GetMovement() != 0);
            
            _playerMovement.StartMoving();
        }
    }
}