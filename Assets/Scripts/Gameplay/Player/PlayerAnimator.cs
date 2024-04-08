using UnityEngine;

namespace Infrastructure.States
{
    public class PlayerAnimator
    {
        private Animator _animator;
        
        private readonly int _jumpId = Animator.StringToHash("Jump");
        
        public void Jump() => 
            _animator.SetTrigger(_jumpId);

        public void SetStickman(Animator animator) => 
            _animator = animator;
    }
}