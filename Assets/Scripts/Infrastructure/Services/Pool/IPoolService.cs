using Gameplay.Level;
using UnityEngine;

namespace Infrastructure.Services.Pool
{
    public interface IPoolService : IService
    {
        ChunkPoolsHolder ChunkChunkPools { get; }
        PathPool<Transform> SimpleCubes { get; }
        PathPool<Transform> PlayerCubes { get; }
    }
}