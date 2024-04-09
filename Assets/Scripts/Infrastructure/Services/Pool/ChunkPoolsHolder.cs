using System.Collections.Generic;
using Infrastructure.Services.Pool;
using Zenject;

namespace Gameplay.Level
{
    public class ChunkPoolsHolder
    {
        private readonly Dictionary<string, BasePool<Chunk>> _pools = new();

        public ChunkPoolsHolder(List<Chunk> chunks, IInstantiator instantiator)
        {
            foreach (var chunk in chunks)
            {
                BasePool<Chunk> pool = new PrefabInjectPool<Chunk>(chunk, instantiator);
                
                _pools.Add(chunk.Id, pool);
            }
        }

        public BasePool<Chunk> GetPoolById(string id) => 
            _pools[id];

        public void SpawnParents(string name)
        {
            foreach (KeyValuePair<string, BasePool<Chunk>> pool in _pools)
            {
                pool.Value.SpawnParent(name);
            }
        }
    }
}