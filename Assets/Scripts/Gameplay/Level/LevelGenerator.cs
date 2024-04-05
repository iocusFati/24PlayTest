using System.Collections.Generic;
using DG.Tweening;
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
            
            GenerateFirstChunks();
        }
        
        public void GenerateFirstChunks()
        {
            _spawnedChunks.Add(_firstChunk);
            _firstChunk.gameObject.SetActive(true);
            
            for (int i = 0; i < _chunksConfig.ChunksInTheBeginning - 1; i++)
            {
                 SpawnNextChunk(false);
            }
        }

        public void SpawnNextChunk(bool withAnimation)
        {
            Chunk chunk = GetChunk();

            if (withAnimation) 
                ChunkMoveAppear(chunk);
            else
                SetChunkTransform(chunk);
            
            UpdateSpawnedChunks(chunk);
        }

        private void ChunkMoveAppear(Chunk chunk)
        {
            Vector3 chunkSpawnPosition = _spawnedChunks[^1].End;
            chunk.transform.SetPositionAndRotation(
                chunkSpawnPosition - _chunksConfig.SpawnOffset, _firstChunk.transform.rotation);

            chunk.transform.DOMove(chunkSpawnPosition, _chunksConfig.MoveToSpawnPointDuration);

        }

        public void HideAllChunks()
        {
            int startIndex = _firstChunk.gameObject.activeSelf ? 1 : 0;
            
            for (int index = startIndex ; index < _spawnedChunks.Count;) 
                ReleaseChunk(index);
            
            Debug.Log("Spawned chunks " + _spawnedChunks.Count);
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
                _spawnedChunks.RemoveAt(0);
                
                return;
            }

            ReleaseChunk(0);
        }

        private void ReleaseChunk(int index)
        {
            BasePool<Chunk> pool = _chunkChunkPools.GetPoolById(_spawnedChunks[index].Id);
            pool.Release(_spawnedChunks[index]);
            
            _spawnedChunks.RemoveAt(index);
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
            chunk.transform.SetPositionAndRotation(
                chunkSpawnPosition, _firstChunk.transform.rotation);
        }
    }
}