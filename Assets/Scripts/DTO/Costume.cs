using System.Collections.Generic;

public class Costume
{
    public int CostumeId { get; set; }
    public string Name { get; set; }
    public string NameRu { get; set; }
    public int CostumeAmount { get; set; }
    public List<InventoryCard> Cards { get; set; }

    public Costume()
    {
        Cards = new List<InventoryCard>();
    }
}
