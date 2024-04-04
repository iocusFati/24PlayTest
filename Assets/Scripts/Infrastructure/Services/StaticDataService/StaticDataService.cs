using Infrastructure.AssetProviderService;
using Infrastructure.StaticData.ChunkData;
using Infrastructure.StaticData.PlayerData;
using UnityEngine;

namespace Infrastructure.Services.StaticDataService
{
    public class StaticDataService : IStaticDataService
    {
        public PlayerStaticData PlayerData { get; set; }
        public ChunksConfig ChunkConfig { get; private set; }

        public void Initialize()
        {
            InitializeChunkConfig();
        }

        private void InitializeChunkConfig() => 
            ChunkConfig = Resources.Load<ChunksConfig>(AssetPaths.ChunkConfig);
    }
}