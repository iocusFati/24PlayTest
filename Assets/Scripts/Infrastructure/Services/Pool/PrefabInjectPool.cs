using UnityEngine;
using Zenject;

namespace Infrastructure.Services.Pool
{
    public class PrefabInjectPool<TPoolable> : BasePool<TPoolable> where TPoolable : MonoBehaviour
    {
        private readonly TPoolable _prefab;
        private readonly IInstantiator _instantiator;

        public PrefabInjectPool(TPoolable prefab, IInstantiator instantiator)
        {
            _prefab = prefab;
            _instantiator = instantiator;
        }

        protected override TPoolable Spawn() => 
            _instantiator.InstantiatePrefabForComponent<TPoolable>(_prefab, _parent);
    }
}