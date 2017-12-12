using System.Collections;
using System.Collections.Generic;
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
        if(info.FirstPlayer.Id == LoginManager.Instance.User.Id)
        {
            Title.text = LocalizationManager.GetLocalizedValue("duelwin");//"Поздравляем!\nВы победили!";
            PrizeText.text = LocalizationManager.GetLocalizedValue("yourprize");//"Ваш приз:";

        }
        else
        {
            Title.text = LocalizationManager.GetLocalizedValue("duelloose");//"Сожалеем!\nВы проиграли!";
            PrizeText.text = LocalizationManager.GetLocalizedValue("opponentprize");//"Соперник получил:";
        }
        Prize.text = info.Prize.ToString();
        Winner.SetDuelPlayerObject(info.FirstPlayer);
        Loser.SetDuelPlayerObject(info.SecondPlayer);
        gameObject.SetActive(true);
    }
}
