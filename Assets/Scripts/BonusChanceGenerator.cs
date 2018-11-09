using UnityEngine;

public class BonusChanceGenerator : Singleton<BonusChanceGenerator>
{
    [SerializeField]
    public int _bonusChance;
    [SerializeField]
    public int _boxChance = 1;

    public static bool GenerateBox()
    {
        return Random.Range(0, 100) <= Instance._boxChance;
    }

    public static bool GenerateBonus()
    {
        return Random.Range(0, 100) <= Instance._bonusChance;
    }
}
