using Cysharp.Threading.Tasks;
using Gameplay.Level;
using Infrastructure.Services.Input;
using Infrastructure.Services.StaticDataService;
using Infrastructure.StaticData.PlayerData;
using UnityEngine;
using Utils;
using Zenject;

namespace Infrastructure.States
{
    public class Player : MonoBehaviour
    {
        private IInputService _inputService;

        private LevelGenerator _levelGenerator;
        private PlayerConfig _playerConfig;
        
        private PlayerMovement _playerMovement;

        [Inject]
        public void Construct(LevelGenerator levelGenerator, IStaticDataService staticData, IInputService inputService)
        {
            _levelGenerator = levelGenerator;
            _playerConfig = staticData.PlayerConfig;
            _inputService = inputService;

            _playerMovement = new PlayerMovement(inputService, transform, _playerConfig);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tags.Wall))
            {
                _levelGenerator.SpawnNextChunk();
            }
        }

        public void Initialize()
        {
            StartMovementOnGetMovement().Forget();
        }

        private async UniTaskVoid StartMovementOnGetMovement()
        {
            await UniTask.WaitUntil(() => _inputService.GetMovement() != 0);
            
            _playerMovement.StartMoving();
        }
    }
}