using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class DuelResult : MonoBehaviour
    {
        public Text Title;
        public Text PrizeText;
        public Text Prize;
        public Text IceCream;
        public FriendObject Winner;
        public FriendObject Loser;

        public void SetResult(DuelRes model)
        {
            IceCream.text = LoginManager.User.IceCream.ToString();

            if (model.Status == DuelResultStatus.Win)
            {
                Title.text = LocalizationManager.GetLocalizedValue("duelwin");//"Поздравляем!\nВы победили!";
                PrizeText.text = LocalizationManager.GetLocalizedValue("yourprize");//"Ваш приз:";
            }
            else if (model.Status == DuelResultStatus.Loose)
            {
                Title.text = LocalizationManager.GetLocalizedValue("duelloose");//"Сожалеем!\nВы проиграли!";
                PrizeText.text = LocalizationManager.GetLocalizedValue("opponentprize");//"Соперник получил:";
            }
            else
            {
                //TODO: Add draw text and prize
            }

            Prize.text = model.Prize.ToString();
            Winner.SetDuelPlayerObject(model.Winner);
            Loser.SetDuelPlayerObject(model.Looser);
            gameObject.SetActive(true);
        }
    }
}
