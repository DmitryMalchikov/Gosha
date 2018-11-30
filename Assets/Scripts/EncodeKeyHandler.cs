using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
