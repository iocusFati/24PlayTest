using System;
using UnityEngine;
using Utils;

namespace Infrastructure.States
{
    public class PlayerMainCube : PlayerCube
    {
        public event Action OnWallTriggerEntered;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.WallTrigger)) 
                OnWallTriggerEntered.Invoke();
        }
    }
}