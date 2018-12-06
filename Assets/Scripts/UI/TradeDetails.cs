using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TradeDetails : MonoBehaviour
    {
        public TradeOfferModel info;

        public Text Title;
        public Button AcceptBtn;
        public Button DeclineBtn;

        public Text FirstUserName;
        public Text SecondUserName;

        public Image FirstUserItemImg;
        public Image SecondUserItemImg;

        public Text FirstUserItemName;
        public Text SecondUserItemName;

        public void Accept()
        {
            TradeManager.Instance.AcceptTradeAsync(info.Id);
            LoginManager.Instance.GetUserInfoAsync();
        }

        public void Decline()
        {
            TradeManager.Instance.DeclineTradeAsync(info.Id);
            LoginManager.Instance.GetUserInfoAsync();
        }

        public void SetDetails(TradeOfferModel model)
        {
            info = model;

            if (model.UserId == LoginManager.User.Id)
            {
                Title.text = LocalizationManager.GetLocalizedValue("yourtradeoffer") + model.Nickname;
                FirstUserName.text = LoginManager.User.Nickname;
                SecondUserName.text = model.Nickname;
                AcceptBtn.gameObject.SetActive(false);
            }
            else
            {
                Title.text = model.Nickname + LocalizationManager.GetLocalizedValue("offeredyou");
                FirstUserName.text = model.Nickname;
                SecondUserName.text = LoginManager.User.Nickname;
                AcceptBtn.gameObject.SetActive(true);
            }

            FirstUserItemName.text = ItemName(model.OfferItem);
            SecondUserItemName.text = ItemName(model.RequestItem);
            FirstUserItemImg.sprite = Resources.Load<Sprite>(ItemImageName(model.OfferItem));
            SecondUserItemImg.sprite = Resources.Load<Sprite>(ItemImageName(model.RequestItem));
            gameObject.SetActive(true);
        }

        public string ItemName(InventoryItem item)
        {
            if (item.Amount > 1)
            {
                return string.Format("{0}: {1}", LocalizationManager.GetValue(item.NameRu, item.Name), item.Amount);
            }
            else
            {
                return LocalizationManager.GetValue(item.NameRu, item.Name);
            }
        }

        public string ItemImageName(InventoryItem item)
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
