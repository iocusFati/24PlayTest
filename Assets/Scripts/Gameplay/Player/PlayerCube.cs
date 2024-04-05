using UnityEngine;
using Utils;

namespace Infrastructure.States
{
    public class PlayerCube : MonoBehaviour
    {
        [SerializeField] private bool _isBaseCube;

        private int _wallMask;

        private PlayerStack _playerStack;

        private bool _shouldReactToWall;

        public void Construct(PlayerStack playerStack)
        {
            _playerStack = playerStack;
        }

        private void Awake()
        {
            _wallMask = 1 << 6;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(Tags.Wall) && _shouldReactToWall)
            {
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.WallTrigger))
            {
                CheckForCollision();
                _shouldReactToWall = false;
            }
        }
        
        public void CheckForCollision()
        {
            if (Physics.BoxCast(transform.position - transform.forward * 1.5f, transform.lossyScale * 0.5f, transform.forward,
                    transform.rotation, 4, _wallMask))
            {
                Debug.Log("Collided" + gameObject.name);
                _playerStack.RemoveCube(gameObject);
            }

        }
    }
}