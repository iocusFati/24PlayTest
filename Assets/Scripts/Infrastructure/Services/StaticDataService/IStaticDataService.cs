using Infrastructure.StaticData.ChunkData;
using Infrastructure.StaticData.PlayerData;

namespace Infrastructure.Services.StaticDataService
{
    public interface IStaticDataService : IService
    {
        PlayerConfig PlayerConfig { get; }
        ChunksConfig ChunkConfig { get; }
    }
}