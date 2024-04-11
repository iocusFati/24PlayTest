using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infrastructure.States
{
    public class RagdollHandler : MonoBehaviour
    {
        private List<Rigidbody> _rigidbodies;

        public void Initialize() => 
            _rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());

        public void Enable(bool enable)
        {
            foreach (Rigidbody rb in _rigidbodies) 
                rb.isKinematic = !enable;
        }

        public void Hit(Vector3 position, Vector3 force)
        {
            Rigidbody hitRB = _rigidbodies
                .OrderBy(rigidbody => Vector3.Distance(position, rigidbody.transform.position))
                .First();
            
            hitRB.AddForceAtPosition(force, position, ForceMode.Impulse);
            
        }
    }
}