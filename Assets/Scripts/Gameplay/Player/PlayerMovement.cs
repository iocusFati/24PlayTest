using Infrastructure.Services.Input;
using Infrastructure.StaticData.PlayerData;
using UnityEngine;

namespace Infrastructure.States
{
    public class PlayerMovement : IUpdatable, IFixedUpdatable
    {
        private readonly IInputService _inputService;
        private readonly Transform _playerTransform;
        private readonly PlayerConfig _playerConfig;
        
        private Direction _blockDirection;
        
        private readonly Rigidbody _baseCubeRB;

        private readonly IUpdater _updater;
        
        private bool _isMoving = true;

        public PlayerMovement(IInputService inputService, PlayerConfig playerConfig, Transform playerTransform,
            Rigidbody baseCubeRB, IUpdater updater)
        {
            _inputService = inputService;
            _playerTransform = playerTransform;
            _playerConfig = playerConfig;
            _baseCubeRB = baseCubeRB;
            _updater = updater;
        }

        public void StartMoving()
        {
            if (_isMoving)
            {
                _updater.AddFixedUpdatable(this);
                _updater.AddUpdatable(this);
            }
            
            _isMoving = true;
        }

        public void Update()
        {
            if (!_isMoving)
                return;

            _inputService.GetMovement();
        }

        public void FixedUpdate()
        {
            if (!_isMoving)
                return;
            
            MoveForward();
        }

        public void StopMoving() => 
            _isMoving = false;

        private void MoveForward()
        {
            Vector3 velocity = _baseCubeRB.velocity;
            velocity.z = _playerConfig.Speed * Time.fixedDeltaTime;

            MoveSide(ref velocity);
            velocity += Physics.gravity * _playerConfig.GravityModifier;

            _baseCubeRB.velocity = velocity;
        }

        private void MoveSide(ref Vector3 currentVelocity)
        {
            float movement = _inputService.GetMovement();
            
            currentVelocity.x = movement * _playerConfig.SideSpeed * Time.fixedDeltaTime;
            
            // _baseCubeRB.velocity = currentVelocity;


            // Vector3 playerPos = _playerTransform.position;
            //
            // Vector3 hypotheticalDisplacement = 
            //     playerPos + new Vector3(movement * _playerConfig.SideSpeed * Time.deltaTime, 0, 0);
            //
            // switch (_blockDirection)
            // {
            //     case Direction.None:
            //         if (hypotheticalDisplacement.x > _playerConfig.RightEdgeX)
            //         {
            //             _playerTransform.position += new Vector3(_playerConfig.RightEdgeX - playerPos.x, 0, 0);
            //             _blockDirection = Direction.Right;
            //             
            //             return;
            //         }
            //
            //         if (hypotheticalDisplacement.x < _playerConfig.LeftEdgeX)
            //         {
            //             _playerTransform.position += new Vector3(_playerConfig.LeftEdgeX - playerPos.x, 0, 0);
            //             _blockDirection = Direction.Left;
            //             
            //             return;
            //         }
            //
            //         break;
            //     case Direction.Right:
            //         if (movement > 0)
            //             hypotheticalDisplacement = new Vector3(_playerConfig.RightEdgeX, hypotheticalDisplacement.y, hypotheticalDisplacement.z);
            //         else if (movement < 0)
            //             _blockDirection = Direction.None;
            //         
            //         break;
            //     case Direction.Left:
            //         if (movement < 0)
            //             hypotheticalDisplacement = new Vector3(_playerConfig.LeftEdgeX, hypotheticalDisplacement.y, hypotheticalDisplacement.z);
            //         else if (movement > 0)
            //             _blockDirection = Direction.None;
            //         
            //         break;
            // }
            //
            // _playerTransform.position = hypotheticalDisplacement;

        }
    }

    public enum Direction
    {
        None = 0,
        Right = 1,
        Left = 2
    }
}