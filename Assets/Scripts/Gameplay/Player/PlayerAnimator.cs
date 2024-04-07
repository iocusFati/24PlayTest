using UnityEngine;

namespace Infrastructure.States
{
    public class PlayerAnimator
    {
        private readonly Animator _animator;
        
        private readonly int _jumpId = Animator.StringToHash("Jump");

        public PlayerAnimator(Animator animator)
        {
            _animator = animator;
        }

        public void Jump() => 
            _animator.SetTrigger(_jumpId);
    }
}