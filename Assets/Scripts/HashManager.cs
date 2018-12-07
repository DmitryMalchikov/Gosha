using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class HashManager : MonoBehaviour
    {
        private static string _duelsHash;
        private static string _friendsHash;
        private static string _shopHash;
        private static string _suitsHash;
        private static string _tradesHash;

        public static void SetDuelsHash(string value)
        {
            _duelsHash = value;
            PlayerPrefs.SetString("DuelsHash", _duelsHash);
        }

        public static void SetFriendsHash(string value)
        {
            _friendsHash = value;
            PlayerPrefs.SetString("FriendsHash", _friendsHash);
        }

        public static void SetShopHash(string value)
        {
            _shopHash = value;
            PlayerPrefs.SetString("ShopHash", _shopHash);
        }

        public static void SetSuitsHash(string value)
        {
            _suitsHash = value;
            PlayerPrefs.SetString("SuitsHash", _suitsHash);
        }

        public static void SetTradesHash(string value)
        {
            _tradesHash = value;
            PlayerPrefs.SetString("TradesHash", _tradesHash);
        }

        private static void LoadHashes()
        {
            _friendsHash = PlayerPrefs.GetString("FriendsHash");
            _duelsHash = PlayerPrefs.GetString("DuelsHash");
            _suitsHash = PlayerPrefs.GetString("SuitsHash");
            _shopHash = PlayerPrefs.GetString("ShopHash");
            _tradesHash = PlayerPrefs.GetString("TradesHash");
        }

        private void Start()
        {
            LoadHashes();
        }

        public static bool GetForceUpdate(DataType type)
        {
            string savedHash;

            if (LoginManager.LocalUser)
            {
                return false;
            }

            if (LoginManager.User == null)
            {
                return false;
            }

            switch (type)
            {
                case DataType.Duels:
                    savedHash = _duelsHash;
                    return LoginManager.User.DuelsHash != savedHash;
                case DataType.Friends:
                    savedHash = _friendsHash;
                    return LoginManager.User.FriendsHash != savedHash;
                case DataType.Shop:
                    savedHash = _shopHash;
                    return LoginManager.User.ShopHash != savedHash;
                case DataType.Suits:
                    savedHash = _suitsHash;
                    return LoginManager.User.SuitsHash != savedHash;
                case DataType.Trades:
                    savedHash = _tradesHash;
                    return LoginManager.User.TradesHash != savedHash;
                case DataType.UserInfo:
                    return !LoginManager.LocalUser;
            }

            return true;
        }
    }
}
