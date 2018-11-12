public class TradeOfferModel
{
    public InventoryItem OfferItem { get; set; }
    public InventoryItem RequestItem { get; set; }
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Nickname { get; set; }
}
