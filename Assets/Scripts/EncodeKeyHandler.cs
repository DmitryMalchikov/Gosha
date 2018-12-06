using Assets.Scripts.Gameplay;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class EncodeKeyHandler : Singleton<EncodeKeyHandler>
    {
        [SerializeField]
        private string _shopKey = "sosipisos";

        public static string GetKey(DataType type)
        {
            string pass = string.Empty;

            if (type == DataType.Shop && LoginManager.LocalUser)
            {
                pass = Instance._shopKey;
            }
            else
            {
                pass = GameController.DeviceId;
            }

            return pass;
        }
    }
}
