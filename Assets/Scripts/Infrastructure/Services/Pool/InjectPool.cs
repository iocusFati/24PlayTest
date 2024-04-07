using UnityEngine;
using Zenject;

namespace Infrastructure.Services.Pool
{
    public class InjectPool<TPoolable> : BasePool<TPoolable> where TPoolable : Component
    {
        private readonly string _path;
        private readonly IInstantiator _instantiator;

        public InjectPool(string path, IInstantiator instantiator)
        {
            _path = path;
            _instantiator = instantiator;
        }

        protected override TPoolable Spawn() => 
            _instantiator.InstantiatePrefabResourceForComponent<TPoolable>(_path, _parent);
    }
}