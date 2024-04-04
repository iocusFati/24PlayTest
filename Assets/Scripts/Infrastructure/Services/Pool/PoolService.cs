using System.Collections.Generic;
using System.Linq;
using Gameplay.Level;
using Infrastructure.Services.StaticDataService;
using Infrastructure.StaticData.ChunkData;

namespace Infrastructure.Services.Pool
{
    public class PoolService : IPoolService
    {
        private ChunksConfig _chunkConfig;

        public ChunkPoolsHolder ChunkChunkPools { get; set; }

        public PoolService(IStaticDataService staticData)
        {
            _chunkConfig = staticData.ChunkConfig;
            
            Initialize();
        }

        public void Initialize()
        {
            ChunkChunkPools = new ChunkPoolsHolder(_chunkConfig.ChunkPrefabs);
        }
    }
}