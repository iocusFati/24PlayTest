using UnityEngine;

namespace Infrastructure.StaticData.PlayerData
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "StaticData/Configs/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField] private float _speed;
        
        [Header("Edges")]
        [SerializeField] private float _rightEdgeX;
        [SerializeField] private float _leftEdgeX;

        public float Speed => _speed;

        public float RightEdgeX => _rightEdgeX;

        public float LeftEdgeX => _leftEdgeX;
    }
}