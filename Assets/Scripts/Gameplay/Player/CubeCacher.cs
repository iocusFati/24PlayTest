using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.States
{
    public class CubeCacher
    {
        private readonly Dictionary<GameObject, StackCubeCached> _cachedCubes = new();

        public StackCubeCached Get(GameObject key)
        {
            if (_cachedCubes.TryGetValue(key, out var cachedCube))
                return cachedCube;
            
            
            Rigidbody rb = key.GetComponent<Rigidbody>();
            ConfigurableJoint joint = key.GetComponent<ConfigurableJoint>();
            PlayerCube playerCube = key.GetComponent<PlayerCube>();

            StackCubeCached cacheCube = new StackCubeCached(rb, joint, playerCube);
                
            _cachedCubes.Add(key, cacheCube);

            return cacheCube;
        }

    }
}