using System.Collections.Generic;

public class TradeItemsModel
{
    public List<InventoryCard> Cards { get; set; }
    public List<InventoryItem> Bonuses { get; set; }
    public InventoryItem IceCream { get; set; }

    public TradeItemsModel()
    {
        Cards = new List<InventoryCard>();
        Bonuses = new List<InventoryItem>();
        IceCream = new InventoryItem();
    }
}
