using System.Collections.Generic;

public class ShopModel
{
    public List<ShopCard> Cards { get; set; }
    public List<ShopItem> Bonuses { get; set; }
    public List<ShopItem> Cases { get; set; }
    public List<ShopItem> BonusUpgrades { get; set; }
    public string ShopHash { get; set; }
}
