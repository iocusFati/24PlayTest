using System;
using DG.Tweening;
using Infrastructure.Services.Pool;
using Infrastructure.Services.StaticDataService;
using Infrastructure.StaticData.PlayerData;
using UnityEngine;
using Zenject;

namespace Infrastructure.States
{
    public class PlusOneText : MonoBehaviour
    {
        private PlayerConfig _playerConfig;
        
        private CanvasGroup _canvasGroup;

        [Inject]
        public void Construct(IStaticDataService staticDataService)
        {
            _playerConfig = staticDataService.PlayerConfig;
        }

        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void RaiseText(Vector3 stickmanPosition)
        {
            transform.position = stickmanPosition + _playerConfig.PlusOneTextOffset;
            transform.DOMoveY(transform.position.y + _playerConfig.PlusOneTextRaiseBy, _playerConfig.PlusOneTextRaiseDuration);
            DOTween.To(() => _canvasGroup.alpha, alpha => _canvasGroup.alpha = alpha, 0, _playerConfig.PlusOneTextRaiseDuration);
        }
    }
}