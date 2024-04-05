using System;
using System.Collections.Generic;
using Infrastructure.Services.Pool;
using Infrastructure.StaticData.PlayerData;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;

namespace Infrastructure.States
{
    public class PlayerStack : MonoBehaviour
    {
        [SerializeField] private GameObject _cubePrefab;
        [SerializeField] private Transform _stickman;
        [SerializeField] private Transform _cubeHolder;

        [SerializeField] private List<Transform> _stackedCubes;

        private PlayerConfig _playerConfig;

        private CubeCacher _cubeCacher;
        private Vector3 _stickmanInitialLocalPosition;
        private PathPool<Transform> _simpleCubesPool;

        public event Action<GameObject> OnStacked;
        public event Action<GameObject> OnRemoved;

        public void Construct(PlayerConfig playerConfig, IPoolService poolService)
        {
            _playerConfig = playerConfig;
            _simpleCubesPool = poolService.SimpleCubes;
        }

        private void Awake()
        {
            _cubeCacher = new CubeCacher();
            _stickmanInitialLocalPosition = _stickman.localPosition;
            
            _stackedCubes[0].GetComponent<PlayerCube>().Construct(this);
            // OnStacked?.Invoke(_stackedCubes[0].gameObject);
        }

        [Button]
        public void AddCube()
        {
            _cubeHolder.position += new Vector3(0, _playerConfig.RaiseBy, 0);
            
            GameObject cube = Instantiate(_cubePrefab, _cubeHolder);

            PlayerCube playerCube = cube.GetComponent<PlayerCube>();
            playerCube.Construct(this);

            Transform cubeTransform = cube.transform;
            Transform firstCube = _stackedCubes[0];
            Vector3 firstCubePosition = firstCube.position;

            if (_stackedCubes.Count == 1) 
                SetStickman(cube);

            PlaceBaseBlock();
            SetJoints();

            _stackedCubes.Insert(1, cubeTransform);
            
            OnStacked?.Invoke(cube);

            
            void PlaceBaseBlock()
            {
                float cubeSpawnPosY = firstCubePosition.y - firstCube.lossyScale.y * 0.5f - _playerConfig.CubeHeightOffset;

                firstCube.position = new Vector3(firstCubePosition.x, cubeSpawnPosY, firstCubePosition.z);
                cubeTransform.position = firstCubePosition;
            }

            void SetJoints()
            {
                Rigidbody spawnedRB = _cubeCacher.Get(cube).CubeRB;
                Rigidbody firstRB = _cubeCacher.Get(_stackedCubes[0].gameObject).CubeRB;
                ConfigurableJoint spawnedCubeJoint = _cubeCacher.Get(cube).Joint;

                if (_stackedCubes.Count > 1)
                {
                    ConfigurableJoint secondCubeJoint = _stackedCubes[1].GetComponent<ConfigurableJoint>();
                    secondCubeJoint.connectedBody = spawnedRB;
                }
                
                spawnedCubeJoint.connectedBody = firstRB;
            }
        }

        private void SetStickman(GameObject cube)
        {
            _stickman.SetParent(cube.transform);
            _stickman.localPosition = _stickmanInitialLocalPosition;
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

        private void RemoveBaseCube()
        {
            Vector3 secondCubePosition = _stackedCubes[1].position;
            
            DisableCube(1);
            _stackedCubes.RemoveAt(1);

            Vector3 simpleCubePosition = _stackedCubes[0].position;
            
            _stackedCubes[0].position = secondCubePosition;
            _cubeCacher.Get(_stackedCubes[0].gameObject).PlayerCube.CheckForCollision();

            Transform simpleCube = _simpleCubesPool.Get();
            simpleCube.position = simpleCubePosition;
                
            if (_stackedCubes.Count > 1) 
                SetJoints();

            void SetJoints()
            {
                ConfigurableJoint secondJoint = _cubeCacher.Get(_stackedCubes[1].gameObject).Joint;
                Rigidbody baseCubeRB = _cubeCacher.Get(_stackedCubes[0].gameObject).CubeRB;
                
                secondJoint.connectedBody = baseCubeRB;
            }
        }

        private void DisableCube(int index)
        {
            if (_stackedCubes.Count == 2) 
                SetStickman(_stackedCubes[0].gameObject);
            
            _stackedCubes[index].gameObject.SetActive(false);
        }

        private void RemoveCubeWithIndex(int cubeIndex)
        {
            ReplaceStackedCubeWithSimpleOne(cubeIndex);

            if (_stackedCubes.Count - 1 != cubeIndex)
            {
                StackCubeCached cubeCachedAboveIndex = _cubeCacher.Get(_stackedCubes[cubeIndex + 1].gameObject);
                StackCubeCached cubeCachedBelowIndex = _cubeCacher.Get(_stackedCubes[cubeIndex - 1].gameObject);

                SetJointYDrive(cubeCachedAboveIndex);

                cubeCachedAboveIndex.Joint.connectedBody = cubeCachedBelowIndex.CubeRB;
            }
            
            // OnRemoved.Invoke(_stackedCubes[cubeIndex].gameObject);
            _stackedCubes.RemoveAt(cubeIndex);
        }

        private void ReplaceStackedCubeWithSimpleOne(int cubeIndex)
        {
            DisableCube(cubeIndex);
            
            Transform simpleCube = _simpleCubesPool.Get();
            simpleCube.position = _stackedCubes[cubeIndex].position;
        }

        private void SetJointYDrive(StackCubeCached cubeCachedAbove)
        {
            JointDrive jointYDrive = cubeCachedAbove.Joint.yDrive;
            JointDrive initialPositionSpring = jointYDrive;

            jointYDrive.positionSpring = _playerConfig.CubeReconnectSpring;
            jointYDrive.positionDamper = _playerConfig.CubeReconnectDamper;
            jointYDrive.maximumForce = _playerConfig.CubeReconnectMaximumForce;

            cubeCachedAbove.Joint.yDrive = jointYDrive;
            
            cubeCachedAbove.CubeRB.OnCollisionEnterAsObservable()
                .Where(other => other.gameObject.CompareTag(Tags.Player))
                .Subscribe(_ => cubeCachedAbove.Joint.yDrive = initialPositionSpring);
        }

    }
}