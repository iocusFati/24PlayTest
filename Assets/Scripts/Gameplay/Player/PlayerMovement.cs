using Infrastructure.Services.Input;
using Infrastructure.StaticData.PlayerData;
using UnityEngine;

namespace Infrastructure.States
{
    public class PlayerMovement : IUpdatable, IFixedUpdatable
    {
        private readonly IInputService _inputService;
        private readonly PlayerConfig _playerConfig;
        
        private readonly Rigidbody _baseCubeRB;

        private readonly IUpdater _updater;
        
        private bool _isMoving = true;

        public PlayerMovement(IInputService inputService, PlayerConfig playerConfig,
            Rigidbody baseCubeRB, IUpdater updater)
        {
            _inputService = inputService;
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

        public void StopMoving()
        {
            _baseCubeRB.velocity = Vector3.zero + Physics.gravity * _playerConfig.GravityModifier;
            _isMoving = false;
        }

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
        }
    }
}