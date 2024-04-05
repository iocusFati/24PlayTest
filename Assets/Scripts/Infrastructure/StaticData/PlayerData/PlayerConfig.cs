using UnityEngine;
using UnityEngine.Serialization;

namespace Infrastructure.StaticData.PlayerData
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "StaticData/Configs/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Speed")]
        [FormerlySerializedAs("_speed")] [SerializeField] private float _sideSpeed;
        [SerializeField] private float _speed;
        
        [Header("Edges")]
        [SerializeField] private float _rightEdgeX;
        [SerializeField] private float _leftEdgeX;
        
        [Header("Stacking")]
        [SerializeField] private float _cubeHeightOffset = 0.1f;
        [SerializeField] private float _raiseBy = 0.9f;
        
        [Header("Joints")]
        [SerializeField] private float _cubeReconnectSpring;
        [SerializeField] private float _cubeReconnectDamper;
        [SerializeField] private float _cubeReconnectMaximumForce;

        public float SideSpeed => _sideSpeed;

        public float RightEdgeX => _rightEdgeX;

        public float LeftEdgeX => _leftEdgeX;

        public float CubeHeightOffset => _cubeHeightOffset;

        public float RaiseBy => _raiseBy;

        public float Speed => _speed;

        public float CubeReconnectSpring => _cubeReconnectSpring;

        public float CubeReconnectDamper => _cubeReconnectDamper;

        public float CubeReconnectMaximumForce => _cubeReconnectMaximumForce;
    }
}