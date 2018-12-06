using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class Ramp : MonoBehaviour
    {
        public static bool PlayerOnRamp { get; private set; }

        static Ramp()
        {
            PlayerOnRamp = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerOnRamp = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerOnRamp = false;
            }
        }

        public static void StickPlayerToGround()
        {
            if (PlayerOnRamp)
            {
                PlayerController.Instance.StickToGround();
            }
        }
    }
}
