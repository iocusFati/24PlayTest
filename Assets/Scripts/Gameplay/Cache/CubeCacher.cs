using UnityEngine;

namespace Infrastructure.States
{
    public class CubeCacher : BaseCacher<StackCubeCached>
    {
        protected override StackCubeCached CreateCachedValue(GameObject keyBeingCached)
        {
            Rigidbody rigidbody = keyBeingCached.GetComponent<Rigidbody>();
            ConfigurableJoint joint = keyBeingCached.GetComponent<ConfigurableJoint>();
            PlayerCube playerCube = keyBeingCached.GetComponent<PlayerCube>();

            return new StackCubeCached(rigidbody, joint, playerCube);
        }
    }
}