using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infrastructure.Services.Pool;
using Infrastructure.StaticData.PlayerData;
using UnityEngine;

namespace Infrastructure.States
{
    public class PlayerCollisions
    {
        private Transform _stickman;
        private readonly Transform _cubesHolder;
        private readonly CubeCacher _cubeCacher;
        private readonly PlayerConfig _playerConfig;
        private readonly IStackedCubes _stackedCubes;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly BasePool<Transform> _simpleCubesPool;
        private readonly BasePool<Transform> _playerCubePool;

        private Vector3 _stickmanInitialLocalPosition;
        private readonly CancellationTokenSource _tokenSource;

        public event Action OnLost;

        private List<Transform> StackedCubes => _stackedCubes.Cubes;

        public PlayerCollisions(IPoolService poolService, IStackedCubes stackedCubes, ICoroutineRunner coroutineRunner,
            PlayerConfig playerConfig, Transform cubesHolder, CubeCacher cubeCacher)
        {
            _playerConfig = playerConfig;
            _simpleCubesPool = poolService.SimpleCubes;
            _playerCubePool = poolService.PlayerCubes;
            _coroutineRunner = coroutineRunner;
            _cubesHolder = cubesHolder;

            _stackedCubes = stackedCubes;

            _cubeCacher = cubeCacher;
            
            _tokenSource = new CancellationTokenSource();

            PlayerMainCube mainCube = StackedCubes[0].GetComponent<PlayerMainCube>();
            mainCube.OnWallTriggerEntered += AllStackedCubesCheckForCollision;
        }

        public void SetStickman(Transform stickman)
        {
            if (_stickman is null) 
                _stickmanInitialLocalPosition = stickman.localPosition;
            
            _stickman = stickman;
        }

        public void FinishGame()
        {
            _cubeCacher.Get(StackedCubes[0].gameObject).PlayerCube.enabled = false;

            _tokenSource.Cancel();
        }

        private void RemoveCube(GameObject cube)
        {
            int cubeIndex = StackedCubes.IndexOf(cube.transform);
            
            if (cubeIndex != 0)
                RemoveCubeWithIndex(cubeIndex);
            else if (cubeIndex == 0) 
                RemoveBaseCube();
        }

        private void AllStackedCubesCheckForCollision()
        {
            List<Transform> collidedCubes = GetCollidedCubes();

            if (collidedCubes.Count == StackedCubes.Count || collidedCubes.Contains(StackedCubes[^1]))
            {
                _stickman.SetParent(_cubesHolder);
                
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
                
                foreach (var stackedCube in StackedCubes)
                {
                    if (_cubeCacher.Get(stackedCube.gameObject).PlayerCube.CheckForCollision()) 
                        cubes.Add(stackedCube);
                }

                return cubes;
            }

            void ReplaceAllCubesWithSimpleOnes()
            {
                for (int index = 1; index < StackedCubes.Count;)
                {
                    ReplaceStackedCubeWithSimpleOne(index, false);
                    StackedCubes.RemoveAt(index);
                }
            }
        }

        private void RemoveBaseCube()
        {
            if (StackedCubes.Count == 1)
            {
                Lose();

                return;
            }
            
            Vector3 secondCubePosition = StackedCubes[1].position;
            
            DisableCube(1);
            StackedCubes.RemoveAt(1);

            Vector3 simpleCubePosition = StackedCubes[0].position;
            
            StackedCubes[0].position = secondCubePosition;
            _cubeCacher.Get(StackedCubes[0].gameObject).PlayerCube.CheckForCollision();

            Transform simpleCube = _simpleCubesPool.Get();
            simpleCube.position = simpleCubePosition;

            AutoreleaseSimpleCubeAsync(simpleCube).Forget();
                
            if (StackedCubes.Count > 1) 
                SetJoints();

            void SetJoints()
            {
                ConfigurableJoint secondJoint = _cubeCacher.Get(StackedCubes[1].gameObject).Joint;
                Rigidbody baseCubeRB = _cubeCacher.Get(StackedCubes[0].gameObject).CubeRB;
                
                secondJoint.connectedBody = baseCubeRB;
            }
        }

        private void RemoveCubeWithIndex(int cubeIndex)
        {
            ReplaceStackedCubeWithSimpleOne(cubeIndex);

            if (StackedCubes.Count - 1 != cubeIndex)
            {
                StackCubeCached cubeCachedAboveIndex = _cubeCacher.Get(StackedCubes[cubeIndex + 1].gameObject);
                StackCubeCached cubeCachedBelowIndex = _cubeCacher.Get(StackedCubes[cubeIndex - 1].gameObject);

                _coroutineRunner.StartCoroutine(SetJointYDrive(cubeCachedAboveIndex));

                cubeCachedAboveIndex.Joint.connectedBody = cubeCachedBelowIndex.CubeRB;
            }
            
            StackedCubes.RemoveAt(cubeIndex);
        }

        private void Lose() => 
            OnLost.Invoke();

        private async UniTaskVoid AutoreleaseSimpleCubeAsync(Transform simpleCube)
        {
            CancellationToken token = _tokenSource.Token;
            
            await UniTask.Delay(TimeSpan.FromSeconds(_playerConfig.SimpleCubeAutoreleaseTime), DelayType.DeltaTime,
                PlayerLoopTiming.Update, token);
            
            _simpleCubesPool.Release(simpleCube);
        }

        private void DisableCube(int index, bool shouldInfluenceStickman = true)
        {
            if (StackedCubes.Count == 2 && shouldInfluenceStickman) 
                SetStickmanTransform(StackedCubes[0]);
            
            _playerCubePool.Release(StackedCubes[index]);
        }

        private void SetStickmanTransform(Transform parent)
        {
            _stickman.SetParent(parent);
            _stickman.localPosition = _stickmanInitialLocalPosition;
        }

        private void ReplaceStackedCubeWithSimpleOne(int cubeIndex, bool shouldInfluenceStickman = true)
        {
            DisableCube(cubeIndex, shouldInfluenceStickman);
            
            Transform simpleCube = _simpleCubesPool.Get();
            simpleCube.position = StackedCubes[cubeIndex].position;
            
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