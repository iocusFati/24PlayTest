using System;
using Gameplay.Level;
using UnityEngine;

namespace Infrastructure.States
{
    public class Player : MonoBehaviour
    {
        private LevelGenerator _levelGenerator;

        private void OnTriggerExit(Collider other)
        {
            _levelGenerator.SpawnNextChunk();
        }
    }
}