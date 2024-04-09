using System.Collections;
using Infrastructure.StaticData.PlayerData;
using UniRx;
using UnityEngine;

namespace Infrastructure.States
{
    public class PlayerCubesJointHandler
    {
        private readonly IStackedCubes _stackedCubes;
        private readonly PlayerConfig _playerConfig;
        private readonly CubeCacher _cubeCacher;
        private readonly ICoroutineRunner _coroutineRunner;
        private ReactiveCollection<Transform> Cubes => _stackedCubes.Cubes;

        public PlayerCubesJointHandler(IStackedCubes stackedCubes, CubeCacher cubeCacher, PlayerConfig playerConfig, ICoroutineRunner coroutineRunner)
        {
            _stackedCubes = stackedCubes;
            _playerConfig = playerConfig;
            _coroutineRunner = coroutineRunner;
            _cubeCacher = cubeCacher;
        }

        public void Initialize()
        {
            Cubes.ObserveCountChanged()
                .Where(count => count > 1)
                .Subscribe(SetNewJointDrives);
        }

        public void SimulateGravityFor(int cubeIndex)
        {
            StackCubeCached cubeCached = _cubeCacher.Get(Cubes[cubeIndex].gameObject);

            _coroutineRunner.StartCoroutine(SetGravityJointYDrive(cubeCached));
        }

        private void SetNewJointDrives(int cubeCount)
        {
            float newSpring = GetSpringForce(cubeCount);

            for (var index = 1; index < cubeCount; index++)
            {
                Transform cube = Cubes[index];
                StackCubeCached cubeCached = _cubeCacher.Get(cube.gameObject);

                if (!cubeCached.DontAffectJoint) 
                    SetDriveSpringY(cubeCached, newSpring);
            }
        }

        private float GetSpringForce(int cubeCount) => 
            _playerConfig.BaseCubeSpring + _playerConfig.AdditionalSpringPerCube * (cubeCount - 2);

        private void SetDriveSpringY(StackCubeCached cube, float newSpring)
        {
            JointDrive jointDrive = cube.Joint.yDrive;
            jointDrive.positionSpring = newSpring;
            
            cube.Joint.yDrive = jointDrive;
        }

        private IEnumerator SetGravityJointYDrive(StackCubeCached cubeCached)
        {
            cubeCached.DontAffectJoint = true;
            
            JointDrive jointYDrive = cubeCached.Joint.yDrive;
            JointDrive initialPositionSpring = jointYDrive;

            if (jointYDrive.positionSpring == _playerConfig.CubeReconnectSpring)
                yield break;
            
            SetJointForGravitySimulation();

            yield return new WaitForSeconds(_playerConfig.CubeReconnectSpringBackToNormalCooldown);
            
            SetToNormalJoint();
            cubeCached.DontAffectJoint = true;


            void SetJointForGravitySimulation()
            {
                jointYDrive.positionSpring = _playerConfig.CubeReconnectSpring;
                jointYDrive.positionDamper = _playerConfig.CubeReconnectDamper;
                jointYDrive.maximumForce = _playerConfig.CubeReconnectMaximumForce;
            
                cubeCached.Joint.yDrive = jointYDrive;
            }

            void SetToNormalJoint()
            {
                jointYDrive.positionSpring = GetSpringForce(Cubes.Count);
                jointYDrive.positionDamper = initialPositionSpring.positionDamper;
                jointYDrive.maximumForce = initialPositionSpring.maximumForce;
            
                cubeCached.Joint.yDrive = initialPositionSpring;
            }
        }

    }
}