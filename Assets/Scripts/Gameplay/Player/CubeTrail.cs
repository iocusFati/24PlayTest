using System;
using UnityEngine;

namespace Infrastructure.States
{
    public class CubeTrail : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _rayDistance;

        private int _groundLayer;
        private TrailRenderer _trail;
        private bool _isEmitting;

        private readonly RaycastHit[] _hitGround = new RaycastHit[1];

        private void Awake()
        {
            _groundLayer = LayerMask.NameToLayer("Ground");
            
            _trail = GetComponent<TrailRenderer>();
        }

        private void Update()
        {
            FollowTarget();
        }

        private void FixedUpdate()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            
            if (Physics.RaycastNonAlloc(ray, _hitGround, _rayDistance, _groundLayer) == 0)
                SetTrailEmitting(false);
            else if (!_isEmitting) 
                SetTrailEmitting(true);
        }

        private void SetTrailEmitting(bool trailEmitting)
        {
            _trail.emitting = trailEmitting;
            _isEmitting = trailEmitting;
        }

        private void FollowTarget()
        {
            Vector3 targetPosition = _target.position;
            transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        }
    }
}