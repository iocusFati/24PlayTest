using Infrastructure.AssetProviderService;
using UnityEngine;

namespace Infrastructure.Services.Pool
{
    public class PrefabPool<TPoolable> : BasePool<TPoolable> where TPoolable : Component
    {
        private readonly TPoolable _prefab;

        public PrefabPool(TPoolable prefab)
        {
            _prefab = prefab;
        }

        protected override TPoolable Spawn() => 
            Object.Instantiate(_prefab);
    }
}