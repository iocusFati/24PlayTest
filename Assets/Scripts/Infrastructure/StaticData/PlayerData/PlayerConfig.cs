using UnityEngine;
using UnityEngine.Serialization;

namespace Infrastructure.StaticData.PlayerData
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "StaticData/Configs/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Physics")] 
        [SerializeField] private float _sideSpeed;
        [SerializeField] private float _speed;
        [SerializeField] private float _gravityModifier;
        [SerializeField] private float _pushStickmanForce;
        
        [Header("Edges")]
        [SerializeField] private float _rightEdgeX;
        [SerializeField] private float _leftEdgeX;
        
        [Header("Stacking")]
        [SerializeField] private double _simpleCubeAutoreleaseTime;
        [SerializeField] private float _cubeHeightOffset = 0.1f;
        [SerializeField] private float _raiseBy = 0.9f;
        [SerializeField] private float _cubeSpawnOffsetY = 1.4f;
        [SerializeField] private int _maxCubes = 10;

        
        [Header("Plus one text")]
        [SerializeField] private Vector3 _plusOneTextOffset;
        [SerializeField] private float _plusOneTextRaiseBy;
        [SerializeField] private float _plusOneTextRaiseDuration;
        
        [Header("Joints")]
        [SerializeField] private float _cubeReconnectSpring;
        [SerializeField] private float _cubeReconnectDamper;
        [SerializeField] private float _cubeReconnectMaximumForce;
        [SerializeField] private float _cubeReconnectSpringBackToNormalCooldown;
        [SerializeField] private float _baseCubeSpring;
        [SerializeField] private float _additionalSpringPerCube;

        public float SideSpeed => _sideSpeed;

        public float RightEdgeX => _rightEdgeX;

        public float LeftEdgeX => _leftEdgeX;

        public float CubeHeightOffset => _cubeHeightOffset;

        public float RaiseBy => _raiseBy;

        public float Speed => _speed;

        public float CubeReconnectSpring => _cubeReconnectSpring;

        public float CubeReconnectDamper => _cubeReconnectDamper;

        public float CubeReconnectMaximumForce => _cubeReconnectMaximumForce;

        public double SimpleCubeAutoreleaseTime => _simpleCubeAutoreleaseTime;

        public float CubeReconnectSpringBackToNormalCooldown => _cubeReconnectSpringBackToNormalCooldown;

        public float GravityModifier => _gravityModifier;

        public Vector3 PlusOneTextOffset => _plusOneTextOffset;

        public float PlusOneTextRaiseBy => _plusOneTextRaiseBy;

        public float PlusOneTextRaiseDuration => _plusOneTextRaiseDuration;

        public float PushStickmanForce => _pushStickmanForce;

        public float BaseCubeSpring => _baseCubeSpring;

        public float AdditionalSpringPerCube => _additionalSpringPerCube;

        public float CubeSpawnOffsetY => _cubeSpawnOffsetY;

        public int MaxCubes => _maxCubes;
    }
}