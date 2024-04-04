using Gameplay.Level;

namespace Infrastructure.Services.Pool
{
    public interface IPoolService : IService
    {
        ChunkPoolsHolder ChunkChunkPools { get; }
    }
}