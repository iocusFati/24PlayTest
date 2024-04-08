using UnityEngine;

namespace Infrastructure.States
{
    public class PlayerCube : MonoBehaviour
    {
        private int _wallMask;

        protected PlayerStack _playerStack;

        private bool _shouldReactToWall;

        public void Construct(PlayerStack playerStack)
        {
            _playerStack = playerStack;
        }

        private void Awake()
        {
            _wallMask = 1 << 6;
        }
        
        public bool CheckForCollision()
        {
            return Physics.BoxCast(transform.position - transform.forward * 1.5f, transform.lossyScale * 0.5f, transform.forward,
                transform.rotation, 4, _wallMask);
        }
    }
}