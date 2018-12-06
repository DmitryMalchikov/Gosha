using Assets.Scripts.Managers;
using Assets.Scripts.UI;

namespace Assets.Scripts.DTO
{
    public class DuelResultModel
    {
        public PlayerDuelModel FirstPlayer { get; set; }
        public PlayerDuelModel SecondPlayer { get; set; }
        public int Prize { get; set; }
    }

    public class DuelRes
    {
        public PlayerDuelModel Winner { get; private set; }
        public PlayerDuelModel Looser { get; private set; }
        public DuelResultStatus Status { get; private set; }
        public int Prize { get; private set; }

        public DuelRes(DuelResultModel model)
        {
            Status = GetDuelResultStatus(model);
            SetWinnerAndLooser(model);
            Prize = model.Prize;
        }

        public void SetWinnerAndLooser(DuelResultModel model)
        {
            if (model.FirstPlayer.Result >= model.SecondPlayer.Result)
            {
                Winner = model.FirstPlayer;
                Looser = model.SecondPlayer;
            }
            else
            {
                Looser = model.FirstPlayer;
                Winner = model.SecondPlayer;
            }    
        }

        public DuelResultStatus GetDuelResultStatus(DuelResultModel model)
        {
            if (model.FirstPlayer.Result == model.SecondPlayer.Result)
            {
                return DuelResultStatus.Draw;
            }

            if (model.FirstPlayer.Id == LoginManager.User.Id)
            {
                return GetStatus(model.SecondPlayer.Result, model.FirstPlayer.Result);
            }
            else
            {
                return GetStatus(model.FirstPlayer.Result, model.SecondPlayer.Result);
            }
        }

        public DuelResultStatus GetStatus(int firstResult, int secondResult)
        {
            if (firstResult > secondResult)
            {
                return DuelResultStatus.Loose;
            }
            else
            {
                return DuelResultStatus.Win;
            }
        }
    }
}