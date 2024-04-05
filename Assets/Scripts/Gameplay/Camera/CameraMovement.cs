using System;
using UnityEngine;

namespace Gameplay.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public Transform Target;
        
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _damper = 2f;

        private void LateUpdate()
        {
            Vector3 transformPosition = transform.position;
            
            Vector3 newPosition = new Vector3(transformPosition.x, transformPosition.y, Target.position.z + _offset.z);

            
            transform.position = newPosition;
        }

        public void SetTarget(Transform target)
        {
            Target = target;
            transform.position = Target.position + _offset;
        }
    }
}