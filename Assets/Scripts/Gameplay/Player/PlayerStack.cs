using Infrastructure.Services.Pool;
using Infrastructure.StaticData.PlayerData;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Infrastructure.States
{
    public class PlayerStack
    {
        private Transform _stickman;
        private readonly Transform _cubeHolder;

        private Vector3 _stickmanInitialLocalPosition;
        private readonly Vector3 _cubeHolderInitialLocalPosition;

        private readonly CubeCacher _cubeCacher;
        private readonly PlayerConfig _playerConfig;
        private readonly BasePool<Transform> _playerCubePool;
        private readonly InjectPool<PlusOneText> _plusOneTextPool;
        private readonly ParticlePool _stackParticlesPool;

        private readonly IStackedCubes _stackedCubes;
        
        private ReactiveCollection<Transform> StackedCubes => _stackedCubes.Cubes;

        public PlayerStack(IPoolService poolService, IStackedCubes stackedCubes, PlayerConfig playerConfig,
            Transform cubeHolder, CubeCacher cubeCacher)
        {
            _playerConfig = playerConfig;
            _playerCubePool = poolService.PlayerCubes;
            _stackParticlesPool = poolService.StackParticles;
            _plusOneTextPool = poolService.PlusOneText;
            _cubeHolder = cubeHolder;

            _stackedCubes = stackedCubes;
            
            _cubeCacher = cubeCacher;
            _cubeHolderInitialLocalPosition = _cubeHolder.localPosition;

            PlayerMainCube mainCube = StackedCubes[0].GetComponent<PlayerMainCube>();
            mainCube.Construct(this);
        }
        
        public void Initialize() => 
            SetStickmanTransform(StackedCubes[0]);

        public void SetStickman(Transform stickman)
        {
            if (_stickman is null) 
                _stickmanInitialLocalPosition = stickman.localPosition;
            
            _stickman = stickman;
        }

        public void HolderToInitialPosition() => 
            _cubeHolder.localPosition = _cubeHolderInitialLocalPosition;

        [Button]
        public void AddCube()
        {
            StackCubeCached mainCubeCached = _cubeCacher.Get(StackedCubes[0].gameObject);

            if (_playerConfig.MaxCubes > StackedCubes.Count) 
                StackCube();
            
            if (StackedCubes.Count == 2) 
                SetStickmanTransform(StackedCubes[1]);

            _plusOneTextPool.Get().RaiseText(_stickman.position);

            SpawnStackParticle();

            
            void SpawnStackParticle()
            {
                Vector3 cubeBottomLocalPosition = new Vector3(0, -0.5f, 0);
                Vector3 particleSpawnPosition = StackedCubes[^1].TransformPoint(cubeBottomLocalPosition);
                
                ParticleSystem particle = _stackParticlesPool.Get();
                particle.transform.position = particleSpawnPosition;
            }

            Transform PrepareForSpawn(out StackCubeCached secondCubeCached)
            {
                Transform secondCube = StackedCubes[1];
                secondCubeCached = _cubeCacher.Get(secondCube.gameObject);
                secondCubeCached.Joint.connectedBody = null;
                return secondCube;
            }

            Vector3 RaiseSecondCube(Transform secondCube)
            {
                Vector3 secondCubePosition = secondCube.position;
                _cubeHolder.position += new Vector3(0, _playerConfig.RaiseBy, 0);
                return secondCubePosition;
            }

            void StackCube()
            {
                if (StackedCubes.Count > 1)
                {
                    Transform secondCube = PrepareForSpawn(out var secondCubeCached);
                    Vector3 secondCubePosition = RaiseSecondCube(secondCube);
                    Transform cube = CreateCube(secondCubePosition);

                    StackCubeCached spawnedCubeCached = _cubeCacher.Get(cube.gameObject);
                    secondCubeCached.Joint.connectedBody = spawnedCubeCached.CubeRB;
                    spawnedCubeCached.Joint.connectedBody = mainCubeCached.CubeRB;
                }
                else
                {
                    Vector3 cubeSpawnPosition = StackedCubes[0].position + new Vector3(0, _playerConfig.CubeSpawnOffsetY, 0);
                    Transform cube = CreateCube(cubeSpawnPosition);
                
                    StackCubeCached spawnedCubeCached = _cubeCacher.Get(cube.gameObject);
                    spawnedCubeCached.Joint.connectedBody = mainCubeCached.CubeRB;
                }
            }
        }

        private Transform CreateCube(Vector3 spawnPosition)
        {
            Transform cube = SpawnCube();
            
            PlayerCube playerCube = cube.GetComponent<PlayerCube>();
            playerCube.Construct(this);
            
            StackedCubes.Insert(1, cube);
            cube.position = spawnPosition;
            
            return cube;

            Transform SpawnCube()
            {
                Transform spawnCube = _playerCubePool.Get();
                
                spawnCube.SetParent(_cubeHolder);
                return spawnCube;
            }
        }

        private void SetStickmanTransform(Transform parent)
        {
            _stickman.SetParent(parent);
            _stickman.localPosition = _stickmanInitialLocalPosition;
        }
    }
}