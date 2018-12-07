using Assets.Scripts.DTO;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Utils
{
    public static class DTOExtensions
    {
        public static string ItemName(this ShopItem item, int amount = -1)
        {
            if (amount == -1)
            {
                amount = item.Amount;
            }

            if (amount > 1)
            {
                return string.Format("{0}: {1}", LocalizationManager.GetValue(item.NameRu, item.Name), amount);
            }
            else
            {
                return LocalizationManager.GetValue(item.NameRu, item.Name);
            }
        }

        public static string ItemImageName(this InventoryItem item)
        {
            if (item.Name.Contains("Card"))
            {
                return item.Name.AddBrackets();
            }
            else
            {
                return "Bonus" + item.ItemId;
            }
        }
    }
}
