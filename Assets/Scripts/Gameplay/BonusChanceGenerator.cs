using UnityEngine;

public class BonusChanceGenerator : Singleton<BonusChanceGenerator>
{
    [SerializeField]
    public byte _bonusChance;
    [SerializeField]
    public byte _boxChance = 1;

    public static bool GenerateBox()
    {
        return Random.Range(0, 100) <= Instance._boxChance;
    }

    public static bool GenerateBonus()
    {
        return Random.Range(0, 100) <= Instance._bonusChance;
    }
}
