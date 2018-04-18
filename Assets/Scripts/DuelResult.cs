using UnityEngine;
using UnityEngine.UI;

public class DuelResult : MonoBehaviour {

    public Text Title;
    public Text PrizeText;
    public Text Prize;
    public Text IceCream;


    public FriendObject Winner;
    public FriendObject Loser;


    public DuelResultModel info;

    public void SetResult(DuelResultModel model)
    {
        IceCream.text = LoginManager.Instance.User.IceCream.ToString();
        info = model;
		bool win = info.FirstPlayer.Id == LoginManager.Instance.User.Id ? info.FirstPlayer.Result > info.SecondPlayer.Result : info.FirstPlayer.Result < info.SecondPlayer.Result;
		if (win) {
			Title.text = LocalizationManager.GetLocalizedValue ("duelwin");//"Поздравляем!\nВы победили!";
			PrizeText.text = LocalizationManager.GetLocalizedValue ("yourprize");//"Ваш приз:";
		} else if (info.FirstPlayer.Result != info.SecondPlayer.Result) {
			Title.text = LocalizationManager.GetLocalizedValue ("duelloose");//"Сожалеем!\nВы проиграли!";
			PrizeText.text = LocalizationManager.GetLocalizedValue ("opponentprize");//"Соперник получил:";
		} else {
			//TODO: Add draw text and prize
		}
        Prize.text = info.Prize.ToString();
        Winner.SetDuelPlayerObject(info.FirstPlayer);
        Loser.SetDuelPlayerObject(info.SecondPlayer);
        gameObject.SetActive(true);
    }
}
