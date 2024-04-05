using System.Collections.Generic;
using System.Linq;
using Gameplay.Level;
using Infrastructure.AssetProviderService;
using Infrastructure.Services.StaticDataService;
using Infrastructure.StaticData.ChunkData;
using UnityEngine;

namespace Infrastructure.Services.Pool
{
    public class PoolService : IPoolService
    {
        private readonly ChunksConfig _chunkConfig;
        private IAssets _assets;

        public ChunkPoolsHolder ChunkChunkPools { get; set; }
        public PathPool<Transform> SimpleCubes { get; set; }

        public PoolService(IStaticDataService staticData, IAssets assets)
        {
            _chunkConfig = staticData.ChunkConfig;
            _assets = assets;
            
            Initialize();
        }

        public void Initialize()
        {
            InitializeChunksPool();
            InitializeSimpleCubes();
        }

        private void InitializeSimpleCubes() => 
            SimpleCubes = new PathPool<Transform>(AssetPaths.SimpleCube, _assets);

        private void InitializeChunksPool() => 
            ChunkChunkPools = new ChunkPoolsHolder(_chunkConfig.ChunkPrefabs);
    }
}