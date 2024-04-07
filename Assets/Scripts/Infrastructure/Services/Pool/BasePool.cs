using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Infrastructure.Services.Pool
{
    public delegate void Release();
    
    public abstract class BasePool<TPoolable> where TPoolable : Component
    {
        protected Transform _parent;
        
        private IObjectPool<TPoolable> _blockPool;

        private readonly List<TPoolable> _activePoolables = new();

        private IObjectPool<TPoolable> Pool
        {
            get
            {
                return _blockPool ??= new ObjectPool<TPoolable>(
                    Spawn,
                    poolable => { poolable.gameObject.SetActive(true); }, 
                    poolable => { poolable.gameObject.SetActive(false); },
                    poolable => { Object.Destroy(poolable.gameObject); });
            }
        }

        protected abstract TPoolable Spawn();

        public TPoolable Get()
        {
            TPoolable poolable = Pool.Get();
            _activePoolables.Add(poolable);

            return poolable;
        }

        public void Release(TPoolable poolable)
        {
            if (!_activePoolables.Contains(poolable))
                return;
            
            Pool.Release(poolable);
            _activePoolables.Remove(poolable);
        }

        public void ReleaseAll()
        {
            foreach (var poolable in _activePoolables)
            {
                ReleaseWithoutRemove(poolable);
            }            
            _activePoolables.Clear();
        }

        private void ReleaseWithoutRemove(TPoolable poolable) => 
            _blockPool.Release(poolable);

        public void SpawnParent(string name)
        {
            _parent = new GameObject(name).transform;
        }
    }
}