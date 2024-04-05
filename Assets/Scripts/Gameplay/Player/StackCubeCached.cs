using UnityEngine;

namespace Infrastructure.States
{
    public class StackCubeCached
    {
        public Rigidbody CubeRB { get; set; }
        public ConfigurableJoint Joint { get; }
        public PlayerCube PlayerCube { get; }

        public StackCubeCached(Rigidbody rigidbody, ConfigurableJoint joint, PlayerCube playerCube)
        {
            CubeRB = rigidbody;
            Joint = joint;
            PlayerCube = playerCube;
        }

    }
}