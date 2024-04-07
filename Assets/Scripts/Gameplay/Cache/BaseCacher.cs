using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.States
{
    public class BaseCacher<TCached>
    {
        private readonly Dictionary<GameObject, TCached> _cachedCubes = new();

        public virtual TCached Get(GameObject keyBeingCached)
        {
            if (_cachedCubes.TryGetValue(keyBeingCached, out var cachedCube))
                return cachedCube;


            TCached cachedValue = CreateCachedValue(keyBeingCached);
            _cachedCubes.Add(keyBeingCached, cachedValue);

            return cachedValue;
        }

        protected virtual TCached CreateCachedValue(GameObject keyBeingCached) => 
            keyBeingCached.GetComponent<TCached>();
    }
}