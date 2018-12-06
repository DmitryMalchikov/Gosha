using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class BonusChanceGenerator : Singleton<BonusChanceGenerator>
    {
        [SerializeField]
        private byte _bonusChance;
        [SerializeField]
        private byte _boxChance = 1;

        public static bool GenerateBox()
        {
            return Random.Range(0, 100) <= Instance._boxChance;
        }

        public static bool GenerateBonus()
        {
            return Random.Range(0, 100) <= Instance._bonusChance;
        }
    }
}
