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
        public PathPool<Transform> PlayerCubes { get; set; }

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
            InitializePlayerCubes();
        }

        private void InitializeSimpleCubes() => 
            SimpleCubes = new PathPool<Transform>(AssetPaths.SimpleCube, _assets);
        
        private void InitializePlayerCubes() => 
            PlayerCubes = new PathPool<Transform>(AssetPaths.PlayerCube, _assets);

        private void InitializeChunksPool() => 
            ChunkChunkPools = new ChunkPoolsHolder(_chunkConfig.ChunkPrefabs);
    }
}