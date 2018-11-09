using UnityEngine;

public class HashManager : MonoBehaviour
{
    public static string DuelsHash { get; private set; }
    public static string FriendsHash { get; private set; }
    public static string ShopHash { get; private set; }
    public static string SuitsHash { get; private set; }
    public static string TradesHash { get; private set; }

    public static void SetDuelsHash(string value)
    {
        DuelsHash = value;
        PlayerPrefs.SetString("DuelsHash", DuelsHash);
    }

    public static void SetFriendsHash(string value)
    {
        FriendsHash = value;
        PlayerPrefs.SetString("FriendsHash", FriendsHash);
    }

    public static void SetShopHash(string value)
    {
        ShopHash = value;
        PlayerPrefs.SetString("ShopHash", ShopHash);
    }

    public static void SetSuitsHash(string value)
    {
        SuitsHash = value;
        PlayerPrefs.SetString("SuitsHash", SuitsHash);
    }

    public static void SetTradesHash(string value)
    {
        TradesHash = value;
        PlayerPrefs.SetString("TradesHash", TradesHash);
    }

    private static void LoadHashes()
    {
        FriendsHash = PlayerPrefs.GetString("FriendsHash");
        DuelsHash = PlayerPrefs.GetString("DuelsHash");
        SuitsHash = PlayerPrefs.GetString("SuitsHash");
        ShopHash = PlayerPrefs.GetString("ShopHash");
        TradesHash = PlayerPrefs.GetString("TradesHash");
    }

    private void Start()
    {
        LoadHashes();
    }
}
