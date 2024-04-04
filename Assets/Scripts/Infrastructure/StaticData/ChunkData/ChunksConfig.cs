using System.Collections.Generic;
using Gameplay.Level;
using Infrastructure.Services.Pool;
using UnityEngine;
using Utils;

namespace Infrastructure.StaticData.ChunkData
{
    [CreateAssetMenu(fileName = "ChunkConfig", menuName = "StaticData/Configs/ChunkConfig")]
    public class ChunksConfig : ScriptableObject
    {
        [SerializeField] private List<Chunk> _chunks;
        [SerializeField] private int _maxChunksInOneTime;
        [SerializeField] private int _chunksInTheBeginning;

        public int MaxChunksInOneTime => _maxChunksInOneTime;
        public int ChunksInTheBeginning => _chunksInTheBeginning;
        public List<Chunk> ChunkPrefabs => _chunks;

        public Chunk GetRandomChunk() => 
            _chunks.GetRandom();
    }
}