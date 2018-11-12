using UnityEngine;

public class BonusUpgrade
{
    public string BonusName { get; set; }
    public int UpgradeAmount { get; set; }
    public float BonusTime
    {
        get
        {
            return 6 * Mathf.Pow(1.15f, UpgradeAmount);
        }
    }
}
