using Infrastructure.AssetProviderService;
using Infrastructure.StaticData.ChunkData;
using Infrastructure.StaticData.PlayerData;
using UnityEngine;

namespace Infrastructure.Services.StaticDataService
{
    public class StaticDataService : IStaticDataService
    {
        public PlayerConfig PlayerConfig { get; private set; }
        public ChunksConfig ChunkConfig { get; private set; }

        public void Initialize()
        {
            InitializeChunkConfig();
            InitializePlayerConfig();
        }

        private void InitializeChunkConfig() => 
            ChunkConfig = Resources.Load<ChunksConfig>(AssetPaths.ChunkConfig);
        
        private void InitializePlayerConfig() => 
            PlayerConfig = Resources.Load<PlayerConfig>(AssetPaths.PlayerConfig);
    }
}