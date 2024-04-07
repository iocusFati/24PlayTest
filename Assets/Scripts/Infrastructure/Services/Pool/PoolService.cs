using Gameplay.Level;
using Infrastructure.AssetProviderService;
using Infrastructure.Services.StaticDataService;
using Infrastructure.States;
using Infrastructure.StaticData.ChunkData;
using UnityEngine;
using Zenject;

namespace Infrastructure.Services.Pool
{
    public class PoolService : IPoolService
    {
        private readonly ChunksConfig _chunkConfig;
        private readonly IAssets _assets;
        private readonly IInstantiator _instantiator;

        public ChunkPoolsHolder ChunkPools { get; private set; }
        public PathPool<Transform> SimpleCubes { get; private set; }
        public PathPool<Transform> PlayerCubes { get; private set; }
        public ParticlePool StackParticles { get; private set; }
        public InjectPool<PlusOneText> PlusOneText { get; private set; }
        public void SpawnParents()
        {
            ChunkPools.SpawnParents(nameof(ChunkPools));
            SimpleCubes.SpawnParent(nameof(SimpleCubes));
            PlayerCubes.SpawnParent(nameof(PlayerCubes));
            StackParticles.SpawnParent(nameof(StackParticles));
            PlusOneText.SpawnParent(nameof(PlusOneText));
        }

        public PoolService(IStaticDataService staticData, IAssets assets, IInstantiator instantiator)
        {
            _chunkConfig = staticData.ChunkConfig;
            _instantiator = instantiator;
            _assets = assets;
            
            Initialize();
        }

        private void Initialize()
        {
            InitializeChunksPool();
            InitializeSimpleCubes();
            InitializePlayerCubes();
            InitializeStackParticles();
            InitializePlusOneText();
        }

        private void InitializeSimpleCubes() => 
            SimpleCubes = new PathPool<Transform>(AssetPaths.SimpleCube, _assets);
        
        private void InitializePlusOneText() => 
            PlusOneText = new InjectPool<PlusOneText>(AssetPaths.PlusOneText, _instantiator);
        
        private void InitializeStackParticles() => 
            StackParticles = new ParticlePool(AssetPaths.StackParticle, _assets);
        
        private void InitializePlayerCubes() => 
            PlayerCubes = new PathPool<Transform>(AssetPaths.PlayerCube, _assets);

        private void InitializeChunksPool() => 
            ChunkPools = new ChunkPoolsHolder(_chunkConfig.ChunkPrefabs);
    }
}