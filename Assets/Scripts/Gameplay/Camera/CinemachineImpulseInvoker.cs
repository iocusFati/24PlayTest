using System;
using Cinemachine;
using UnityEngine;

namespace Infrastructure
{
    public class CinemachineImpulseInvoker : MonoBehaviour
    {
        private CinemachineImpulseSource _impulseSource;

        private void Awake() => 
            _impulseSource = GetComponent<CinemachineImpulseSource>();

        public void GenerateImpulse() => 
            _impulseSource.GenerateImpulse();
    }
}