using System.Collections.Generic;
using Infrastructure.Services.Pool;

namespace Gameplay.Level
{
    public class ChunkPoolsHolder
    {
        private readonly Dictionary<string, BasePool<Chunk>> _pools = new();

        public ChunkPoolsHolder(List<Chunk> chunks)
        {
            foreach (var chunk in chunks)
            {
                BasePool<Chunk> pool = new PrefabPool<Chunk>(chunk);
                
                _pools.Add(chunk.Id, pool);
            }
        }

        public BasePool<Chunk> GetPoolById(string id) => 
            _pools[id];
    }
}