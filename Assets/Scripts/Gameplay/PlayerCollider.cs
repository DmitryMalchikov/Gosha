using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class PlayerCollider : Singleton<PlayerCollider> {

        [SerializeField]
        private CapsuleCollider _collider;
        [SerializeField]
        private Vector3 _colliderStandCenter = new Vector3();
        [SerializeField]
        private Vector3 _colliderCrouchCenter = new Vector3(0, -0.5f, 0);
        [SerializeField]
        private float _colliderCrouchHeight = .35f;
        [SerializeField]
        private float _colliderStandHeight = .7f;

        static PlayerCollider()
        {
            IsCrouch = false;
        }

        public static bool IsCrouch { get; private set; }

        public static bool ColliderEnabled
        {
            get
            {
                return Instance._collider.enabled;
            }
            set
            {
                Instance._collider.enabled = value;
            }
        }

        private static CapsuleCollider InstanceCollider
        {
            get
            {
                return Instance._collider;
            }
        }

        public static void Crouch()
        {
            InstanceCollider.height = Instance._colliderCrouchHeight;
            InstanceCollider.center = Instance._colliderCrouchCenter;
            IsCrouch = true;
        }

        public static void StandUp()
        {
            InstanceCollider.height = Instance._colliderStandHeight;
            InstanceCollider.center = Instance._colliderStandCenter;
            IsCrouch = false;
        }

        public static void ResetCollider()
        {
            StandUp();
            InstanceCollider.enabled = true;
        }
    }
}
