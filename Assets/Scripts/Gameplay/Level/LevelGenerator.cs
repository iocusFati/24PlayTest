using System.Collections.Generic;
using Infrastructure.Services.Pool;
using Infrastructure.Services.StaticDataService;
using Infrastructure.StaticData.ChunkData;
using UnityEngine;
using Zenject;

namespace Gameplay.Level
{
    public class LevelGenerator
    {
        private readonly ChunkPoolsHolder _chunkChunkPools;
        private readonly ChunksConfig _chunksConfig;

        private Chunk _firstChunk;

        private readonly List<Chunk> _spawnedChunks = new();

        public LevelGenerator(IPoolService poolService, IStaticDataService staticData)
        {
            _chunkChunkPools = poolService.ChunkChunkPools;
            _chunksConfig = staticData.ChunkConfig;
        }
        
        public void Initialize()
        {
            _firstChunk = Object.FindFirstObjectByType<Chunk>();
            _spawnedChunks.Add(_firstChunk);
            
            GenerateFirstChunks();
        }
        
        public void GenerateFirstChunks()
        {
            for (int i = 0; i < _chunksConfig.ChunksInTheBeginning - 1; i++)
            {
                 SpawnNextChunk();
            }
        }

        public void SpawnNextChunk()
        {
            Chunk chunk = GetChunk();

            SetChunkTransform(chunk);
            
            UpdateSpawnedChunks(chunk);
        }

        private void UpdateSpawnedChunks(Chunk chunk)
        {
            _spawnedChunks.Add(chunk);

            if (ChunkCountIsOutOfRange()) 
                ReleaseFirstChunk();
        }

        private bool ChunkCountIsOutOfRange() => 
            _spawnedChunks.Count > _chunksConfig.MaxChunksInOneTime;

        private void ReleaseFirstChunk()
        {
            if (_spawnedChunks[0] == _firstChunk)
            {
                _firstChunk.gameObject.SetActive(false);
                
                return;
            }
            
            BasePool<Chunk> pool = _chunkChunkPools.GetPoolById(_spawnedChunks[0].Id);
            pool.Release(_spawnedChunks[0]);
                
            _spawnedChunks.RemoveAt(0);
        }

        private Chunk GetChunk()
        {
            Chunk randomChunk = _chunksConfig.GetRandomChunk();
            
            BasePool<Chunk> pool = _chunkChunkPools.GetPoolById(randomChunk.Id);
            Chunk chunk = pool.Get();
            
            return chunk;
        }

        private void SetChunkTransform(Chunk chunk)
        {
            Vector3 chunkSpawnPosition = _spawnedChunks[^1].End;
            chunk.transform.SetPositionAndRotation(chunkSpawnPosition, _firstChunk.transform.rotation);
        }
    }
}