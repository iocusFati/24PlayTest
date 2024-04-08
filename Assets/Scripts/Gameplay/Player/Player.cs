using System;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Gameplay.Level;
using Infrastructure.AssetProviderService;
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
    public class Player : MonoBehaviour, IStackedCubes
    {
        [SerializeField] private Transform _stickmanSpawnPoint;
        [SerializeField] private Transform _cubesHolder;
        [SerializeField] private Rigidbody _baseCubeRB;
        [SerializeField] private CubeTrail _cubeTrail;
        [SerializeField] private List<Transform> _cubes;

        private IInputService _inputService;

        private LevelGenerator _levelGenerator;
        private PlayerConfig _playerConfig;
        private IPoolService _poolService;
        private IGameStateMachine _gameStateMachine;
        private IAssets _assets;

        private PlayerMovement _playerMovement;
        private PlayerStack _playerStack;
        private PlayerCollisions _playerCollisions;
        private PlayerAnimator _playerAnimator;

        private Stickman _stickman;

        public List<Transform> Cubes => _cubes;

        [Inject]
        public void Construct(LevelGenerator levelGenerator, IStaticDataService staticData, IInputService inputService,
            IPoolService poolService, IGameStateMachine gameStateMachine, IUpdater updater,
            ICoroutineRunner coroutineRunner, IAssets assets)
        {
            _levelGenerator = levelGenerator;
            _playerConfig = staticData.PlayerConfig;
            _inputService = inputService;
            _poolService = poolService;
            _gameStateMachine = gameStateMachine;
            _assets = assets;
            
            CubeCacher cubeCacher = new CubeCacher();
            _playerMovement = new PlayerMovement(inputService, _playerConfig, transform, _baseCubeRB, updater);
            _playerAnimator = new PlayerAnimator();
            _playerStack = new PlayerStack(_poolService, this, _playerConfig, _cubesHolder, cubeCacher);
            _playerCollisions = new PlayerCollisions(_poolService, this, coroutineRunner, _playerConfig, _cubesHolder, cubeCacher);

            _playerCollisions.OnLost += Lose;
        }

        private void Awake()
        {
            SubscribeForBaseCubeEvents();
            
            CinemachineCore.Instance.GetActiveBrain(0)
                .ActiveVirtualCamera.Follow = _baseCubeRB.transform;
            
            SpawnStickman();
        }

        public void SpawnStickman()
        {
            if (_stickman != null)
            {
                _playerCollisions.OnLost -= _stickman.PushForward;
                Destroy(_stickman.gameObject);
            }

            _stickman = _assets.Instantiate<Stickman>(AssetPaths.Stickman, _baseCubeRB.transform,
                _stickmanSpawnPoint.position, _stickmanSpawnPoint.rotation);
            _stickman.Construct(_playerConfig);
            _stickman.Initialize();
            
            _playerCollisions.OnLost += _stickman.PushForward;
            _stickman.OnLost += Lose;

            _playerCollisions.SetStickman(_stickman.transform);
            _playerStack.SetStickman(_stickman.transform);
            _playerAnimator.SetStickman(_stickman.Animator);
        }

        public void PauseTrail() => 
            _cubeTrail.EnableEmitting(false);

        public void Initialize()
        {
            StartMovementOnGetMovement().Forget();
            _cubeTrail.EnableEmitting(true);
        }

        private void PickUp(Collider other)
        {
            other.gameObject.SetActive(false);
            _playerStack.AddCube();
            _playerAnimator.Jump();
        }

        private void Lose()
        {
            _playerMovement.StopMoving();
            _playerCollisions.FinishGame();
            
            _gameStateMachine.Enter<GameLostState>();
        }


        private void SubscribeForBaseCubeEvents()
        {
            TimeSpan spawnNextChunkTimeSpan = TimeSpan.FromSeconds(1);

            _baseCubeRB.OnTriggerEnterAsObservable()
                .Where(other => other.CompareTag(Tags.Pickup))
                .Subscribe(PickUp);

            SubscribeForNewChunk();

            void SubscribeForNewChunk()
            {
                IObservable<Collider> observable = _baseCubeRB.OnTriggerExitAsObservable()
                    .Where(other => other.CompareTag(Tags.WallTrigger));

                observable
                    .Throttle(spawnNextChunkTimeSpan)
                    .First()
                    .Subscribe(_ =>
                    {
                        _levelGenerator.SpawnNextChunk(true);
                        SubscribeForNewChunk();
                    });
            }
        }

        public void BaseCubeToInitialPosition()
        {
            _playerStack.Initialize();

            _playerStack.HolderToInitialPosition();
            _baseCubeRB.transform.localPosition = Vector3.zero;
        }

        private async UniTaskVoid StartMovementOnGetMovement()
        {
            await UniTask.WaitUntil(() => _inputService.CanStartMoving());
            
            _playerMovement.StartMoving();
        }
    }
}