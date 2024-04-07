using Gameplay.Level;
using Infrastructure.States;
using UnityEngine;

namespace Infrastructure.Services.Pool
{
    public interface IPoolService : IService
    {
        ChunkPoolsHolder ChunkPools { get; }
        PathPool<Transform> SimpleCubes { get; }
        PathPool<Transform> PlayerCubes { get; }
        ParticlePool StackParticles { get; }
        InjectPool<PlusOneText> PlusOneText { get; }
        void SpawnParents();
    }
}