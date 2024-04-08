using System.Collections.Generic;
using Infrastructure.Services.Pool;
using Infrastructure.StaticData.PlayerData;
using Sirenix.OdinInspector;
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
        
        private List<Transform> StackedCubes => _stackedCubes.Cubes;

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
            _cubeHolder.position += new Vector3(0, _playerConfig.RaiseBy, 0);
            
            Transform cube = SpawnCube();

            PlayerCube playerCube = cube.GetComponent<PlayerCube>();
            playerCube.Construct(this);

            Transform cubeTransform = cube.transform;
            Transform firstCube = StackedCubes[0];
            Vector3 firstCubePosition = firstCube.position;

            if (StackedCubes.Count == 1) 
                SetStickmanTransform(cube);

            _plusOneTextPool.Get().RaiseText(_stickman.position);

            SpawnStackParticle();
            PlaceBaseBlock();
            SetJoints();

            StackedCubes.Insert(1, cubeTransform);

            
            void PlaceBaseBlock()
            {
                float cubeSpawnPosY = firstCubePosition.y - firstCube.lossyScale.y * 0.5f - _playerConfig.CubeHeightOffset;

                firstCube.position = new Vector3(firstCubePosition.x, cubeSpawnPosY, firstCubePosition.z);
                cubeTransform.position = firstCubePosition;
            }

            void SetJoints()
            {
                Rigidbody spawnedRB = _cubeCacher.Get(cube.gameObject).CubeRB;
                Rigidbody firstRB = _cubeCacher.Get(StackedCubes[0].gameObject).CubeRB;
                ConfigurableJoint spawnedCubeJoint = _cubeCacher.Get(cube.gameObject).Joint;

                if (StackedCubes.Count > 1)
                {
                    ConfigurableJoint secondCubeJoint = StackedCubes[1].GetComponent<ConfigurableJoint>();
                    secondCubeJoint.connectedBody = spawnedRB;
                }
                
                spawnedCubeJoint.connectedBody = firstRB;
            }

            Transform SpawnCube()
            {
                Transform spawnCube = _playerCubePool.Get();
                
                spawnCube.SetParent(_cubeHolder);
                return spawnCube;
            }

            void SpawnStackParticle()
            {
                Vector3 cubeBottomLocalPosition = new Vector3(0, -0.5f, 0);
                Vector3 particleSpawnPosition = StackedCubes[^1].TransformPoint(cubeBottomLocalPosition);
                
                ParticleSystem particle = _stackParticlesPool.Get();
                particle.transform.position = particleSpawnPosition;
            }
        }

        private void SetStickmanTransform(Transform parent)
        {
            _stickman.SetParent(parent);
            _stickman.localPosition = _stickmanInitialLocalPosition;
        }
    }
}