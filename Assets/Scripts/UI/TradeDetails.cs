using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TradeDetails : MonoBehaviour
    {
        public TradeOfferModel Info;

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
            TradeManager.Instance.AcceptTradeAsync(Info.Id);
            LoginManager.Instance.GetUserInfoAsync();
        }

        public void Decline()
        {
            TradeManager.Instance.DeclineTradeAsync(Info.Id);
            LoginManager.Instance.GetUserInfoAsync();
        }

        public void SetDetails(TradeOfferModel model)
        {
            Info = model;

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

            FirstUserItemName.text = model.OfferItem.ItemName();
            SecondUserItemName.text = model.RequestItem.ItemName();
            FirstUserItemImg.sprite = Resources.Load<Sprite>(model.OfferItem.ItemImageName());
            SecondUserItemImg.sprite = Resources.Load<Sprite>(model.RequestItem.ItemImageName());
            gameObject.SetActive(true);
        }
    }
}
