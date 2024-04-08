using System;
using Infrastructure.StaticData.PlayerData;
using UnityEngine;
using Utils;

namespace Infrastructure.States
{
    public class Stickman : MonoBehaviour
    {
        [SerializeField] private RagdollHandler _ragdollHandler;
        [SerializeField] private Transform _addForcePoint;
        [SerializeField] private Animator _animator;
        
        private PlayerConfig _playerConfig;
        private Collider _collider;
        public event Action OnLost;

        public Animator Animator => _animator;

        public void Construct(PlayerConfig playerConfig)
        {
            _playerConfig = playerConfig;
        }
        
        private void Awake()
        {
            _collider = GetComponent<Collider>();
            
            _ragdollHandler.Initialize();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(Tags.Wall))
            {
                EnableRagdoll(true);
                _collider.enabled = false;
                
                OnLost.Invoke();
            }
        }

        public void Initialize()
        {
            EnableRagdoll(false);
        }

        public void PushForward()
        {
            EnableRagdoll(true);
            _ragdollHandler.Hit(_addForcePoint.position, _addForcePoint.forward * _playerConfig.PushStickmanForce);
        }

        private void EnableRagdoll(bool enable)
        {
            Animator.enabled = !enable;
            _ragdollHandler.Enable(enable);
        }
    }
}