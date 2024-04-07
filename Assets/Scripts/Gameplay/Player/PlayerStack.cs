using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infrastructure.Services.Pool;
using Infrastructure.StaticData.PlayerData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Infrastructure.States
{
    public class PlayerStack : MonoBehaviour
    {
        [SerializeField] private Transform _stickman;
        [SerializeField] private Transform _cubeHolder;

        [SerializeField] private List<Transform> _stackedCubes;

        private Vector3 _stickmanInitialLocalPosition;
        private Vector3 _cubeHolderInitialLocalPosition;

        private CubeCacher _cubeCacher;
        private PlayerConfig _playerConfig;
        private BasePool<Transform> _simpleCubesPool;
        private BasePool<Transform> _playerCubePool;
        private InjectPool<PlusOneText> _plusOneTextPool;
        private ParticlePool _stackParticlesPool;
        
        private CancellationTokenSource _tokenSource;

        public event Action OnLost;

        public void Construct(PlayerConfig playerConfig, IPoolService poolService)
        {
            _playerConfig = playerConfig;
            _simpleCubesPool = poolService.SimpleCubes;
            _playerCubePool = poolService.PlayerCubes;
            _stackParticlesPool = poolService.StackParticles;
            _plusOneTextPool = poolService.PlusOneText;
        }

        private void Awake()
        {
            _cubeCacher = new CubeCacher();
            _stickmanInitialLocalPosition = _stickman.localPosition;
            _cubeHolderInitialLocalPosition = _cubeHolder.localPosition;
            
            _tokenSource = new CancellationTokenSource();

            PlayerMainCube mainCube = _stackedCubes[0].GetComponent<PlayerMainCube>();
            mainCube.Construct(this);
            mainCube.OnWallTriggerEntered += AllStackedCubesCheckForCollision;
        }

        public void Initialize() => 
            SetStickman(_stackedCubes[0]);

        [Button]
        public void AddCube()
        {
            _cubeHolder.position += new Vector3(0, _playerConfig.RaiseBy, 0);
            
            Transform cube = SpawnCube();

            PlayerCube playerCube = cube.GetComponent<PlayerCube>();
            playerCube.Construct(this);

            Transform cubeTransform = cube.transform;
            Transform firstCube = _stackedCubes[0];
            Vector3 firstCubePosition = firstCube.position;

            if (_stackedCubes.Count == 1) 
                SetStickman(cube);

            _plusOneTextPool.Get().RaiseText(_stickman.position);

            SpawnStackParticle();
            PlaceBaseBlock();
            SetJoints();

            _stackedCubes.Insert(1, cubeTransform);

            
            void PlaceBaseBlock()
            {
                float cubeSpawnPosY = firstCubePosition.y - firstCube.lossyScale.y * 0.5f - _playerConfig.CubeHeightOffset;

                firstCube.position = new Vector3(firstCubePosition.x, cubeSpawnPosY, firstCubePosition.z);
                cubeTransform.position = firstCubePosition;
            }

            void SetJoints()
            {
                Rigidbody spawnedRB = _cubeCacher.Get(cube.gameObject).CubeRB;
                Rigidbody firstRB = _cubeCacher.Get(_stackedCubes[0].gameObject).CubeRB;
                ConfigurableJoint spawnedCubeJoint = _cubeCacher.Get(cube.gameObject).Joint;

                if (_stackedCubes.Count > 1)
                {
                    ConfigurableJoint secondCubeJoint = _stackedCubes[1].GetComponent<ConfigurableJoint>();
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
                Vector3 particleSpawnPosition = _stackedCubes[^1].TransformPoint(cubeBottomLocalPosition);
                
                ParticleSystem particle = _stackParticlesPool.Get();
                particle.transform.position = particleSpawnPosition;
            }
        }

        [Button]
        public void RemoveCube(GameObject cube)
        {
            int cubeIndex = _stackedCubes.IndexOf(cube.transform);
            
            if (cubeIndex != 0)
                RemoveCubeWithIndex(cubeIndex);
            else if (cubeIndex == 0) 
                RemoveBaseCube();
        }

        public void HolderToInitialPosition() => 
            _cubeHolder.localPosition = _cubeHolderInitialLocalPosition;

        private void AllStackedCubesCheckForCollision()
        {
            List<Transform> collidedCubes = GetCollidedCubes();

            if (collidedCubes.Count == _stackedCubes.Count)
            {
                _stickman.SetParent(_cubeHolder);
                
                ReplaceAllCubesWithSimpleOnes();
                Lose();
            }
            else
            {
                //place main cube at the last position, so that it couldn't effect linked destroying of other cubes
                collidedCubes.Reverse();
                foreach (var collidedCube in collidedCubes)
                {
                    RemoveCube(collidedCube.gameObject);
                }
            }

            List<Transform> GetCollidedCubes()
            {
                List<Transform> cubes = new List<Transform>();
                
                foreach (var stackedCube in _stackedCubes)
                {
                    if (_cubeCacher.Get(stackedCube.gameObject).PlayerCube.CheckForCollision()) 
                        cubes.Add(stackedCube);
                }

                return cubes;
            }

            void ReplaceAllCubesWithSimpleOnes()
            {
                for (int index = 1; index < _stackedCubes.Count;)
                {
                    ReplaceStackedCubeWithSimpleOne(index, false);
                    _stackedCubes.RemoveAt(index);
                }
            }
        }

        private void SetStickman(Transform parent)
        {
            _stickman.SetParent(parent);
            _stickman.localPosition = _stickmanInitialLocalPosition;
        }

        private void RemoveBaseCube()
        {
            if (_stackedCubes.Count == 1)
            {
                Lose();

                return;
            }
            
            Vector3 secondCubePosition = _stackedCubes[1].position;
            
            DisableCube(1);
            _stackedCubes.RemoveAt(1);

            Vector3 simpleCubePosition = _stackedCubes[0].position;
            
            _stackedCubes[0].position = secondCubePosition;
            _cubeCacher.Get(_stackedCubes[0].gameObject).PlayerCube.CheckForCollision();

            Transform simpleCube = _simpleCubesPool.Get();
            simpleCube.position = simpleCubePosition;

            AutoreleaseSimpleCubeAsync(simpleCube).Forget();
                
            if (_stackedCubes.Count > 1) 
                SetJoints();

            void SetJoints()
            {
                ConfigurableJoint secondJoint = _cubeCacher.Get(_stackedCubes[1].gameObject).Joint;
                Rigidbody baseCubeRB = _cubeCacher.Get(_stackedCubes[0].gameObject).CubeRB;
                
                secondJoint.connectedBody = baseCubeRB;
            }
        }

        private void RemoveCubeWithIndex(int cubeIndex)
        {
            ReplaceStackedCubeWithSimpleOne(cubeIndex);

            if (_stackedCubes.Count - 1 != cubeIndex)
            {
                StackCubeCached cubeCachedAboveIndex = _cubeCacher.Get(_stackedCubes[cubeIndex + 1].gameObject);
                StackCubeCached cubeCachedBelowIndex = _cubeCacher.Get(_stackedCubes[cubeIndex - 1].gameObject);

                StartCoroutine(SetJointYDrive(cubeCachedAboveIndex));

                cubeCachedAboveIndex.Joint.connectedBody = cubeCachedBelowIndex.CubeRB;
            }
            
            _stackedCubes.RemoveAt(cubeIndex);
        }

        private void Lose()
        {
            _cubeCacher.Get(_stackedCubes[0].gameObject).PlayerCube.enabled = false;
                
            _tokenSource.Cancel();
                
            OnLost.Invoke();
        }

        private async UniTaskVoid AutoreleaseSimpleCubeAsync(Transform simpleCube)
        {
            CancellationToken token = _tokenSource.Token;
            
            await UniTask.Delay(TimeSpan.FromSeconds(_playerConfig.SimpleCubeAutoreleaseTime), DelayType.DeltaTime,
                PlayerLoopTiming.Update, token);
            
            _simpleCubesPool.Release(simpleCube);
        }

        private void DisableCube(int index, bool shouldInfluenceStickman = true)
        {
            if (_stackedCubes.Count == 2 && shouldInfluenceStickman) 
                SetStickman(_stackedCubes[0]);
            
            _playerCubePool.Release(_stackedCubes[index]);
        }

        private void ReplaceStackedCubeWithSimpleOne(int cubeIndex, bool shouldInfluenceStickman = true)
        {
            DisableCube(cubeIndex, shouldInfluenceStickman);
            
            Transform simpleCube = _simpleCubesPool.Get();
            simpleCube.position = _stackedCubes[cubeIndex].position;
            
            AutoreleaseSimpleCubeAsync(simpleCube).Forget();
        }

        private IEnumerator SetJointYDrive(StackCubeCached cubeCachedAbove)
        {
            JointDrive jointYDrive = cubeCachedAbove.Joint.yDrive;
            JointDrive initialPositionSpring = jointYDrive;

            if (jointYDrive.positionSpring == _playerConfig.CubeReconnectSpring)
                yield break;
            
            jointYDrive.positionSpring = _playerConfig.CubeReconnectSpring;
            jointYDrive.positionDamper = _playerConfig.CubeReconnectDamper;
            jointYDrive.maximumForce = _playerConfig.CubeReconnectMaximumForce;
            
            cubeCachedAbove.Joint.yDrive = jointYDrive;

            yield return new WaitForSeconds(_playerConfig.CubeReconnectSpringBackToNormalCooldown);

            jointYDrive.positionSpring = initialPositionSpring.positionSpring;
            jointYDrive.positionDamper = initialPositionSpring.positionDamper;
            jointYDrive.maximumForce = initialPositionSpring.maximumForce;
            
            cubeCachedAbove.Joint.yDrive = initialPositionSpring;
        }
    }
}