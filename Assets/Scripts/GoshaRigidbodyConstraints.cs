using UnityEngine;

namespace Assets.Scripts
{
    public class GoshaRigidbodyConstraints
    {
        public const RigidbodyConstraints FreezeExceptJump = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;
        public const RigidbodyConstraints FreezeAll = RigidbodyConstraints.FreezeAll;
    }
}
