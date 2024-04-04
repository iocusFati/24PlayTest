using Infrastructure.Services.Input;
using Infrastructure.StaticData.PlayerData;
using UniRx;
using UnityEngine;

namespace Infrastructure.States
{
    public class PlayerMovement
    {
        private readonly IInputService _inputService;
        private readonly Transform _playerTransform;
        private readonly PlayerConfig _playerConfig;
        
        private Direction _blockDirection;
        
        private readonly CompositeDisposable _sideMoveDisposer;

        public PlayerMovement(IInputService inputService, Transform playerTransform, PlayerConfig playerConfig)
        {
            _inputService = inputService;
            _playerTransform = playerTransform;
            _playerConfig = playerConfig;
            
            _sideMoveDisposer = new CompositeDisposable();
        }

        public void StartMoving()
        {
            Observable.EveryUpdate()
                .Subscribe(_ => MoveSide())
                .AddTo(_sideMoveDisposer);
        }

        private void MoveSide()
        {
            float movement = _inputService.GetMovement();
            Vector3 playerPos = _playerTransform.position;
            
            Vector3 hypotheticalDisplacement = 
                playerPos + new Vector3(movement * _playerConfig.Speed * Time.deltaTime, 0, 0);
            
            switch (_blockDirection)
            {
                case Direction.None:
                    if (hypotheticalDisplacement.x > _playerConfig.RightEdgeX)
                    {
                        _playerTransform.position += new Vector3(_playerConfig.RightEdgeX - playerPos.x, 0, 0);
                        _blockDirection = Direction.Right;
                        
                        return;
                    }

                    if (hypotheticalDisplacement.x < _playerConfig.LeftEdgeX)
                    {
                        _playerTransform.position += new Vector3(_playerConfig.LeftEdgeX - playerPos.x, 0, 0);
                        _blockDirection = Direction.Left;
                        
                        return;
                    }

                    break;
                case Direction.Right:
                    if (movement > 0)
                        hypotheticalDisplacement = new Vector3(_playerConfig.RightEdgeX, hypotheticalDisplacement.y, hypotheticalDisplacement.z);
                    else if (movement < 0)
                        _blockDirection = Direction.None;
                    
                    break;
                case Direction.Left:
                    if (movement < 0)
                        hypotheticalDisplacement = new Vector3(_playerConfig.LeftEdgeX, hypotheticalDisplacement.y, hypotheticalDisplacement.z);
                    else if (movement > 0)
                        _blockDirection = Direction.None;
                    
                    break;
            }
            
            _playerTransform.position = hypotheticalDisplacement;

        }
    }

    public enum Direction
    {
        None = 0,
        Right = 1,
        Left = 2
    }
}