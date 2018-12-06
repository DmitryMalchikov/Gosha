using Assets.Scripts.DTO;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class DuelInfo : TimeCheck, IAvatarSprite
    {
        public Text Name;
        public Text Bet;
        public Image Avatar;

        protected DuelModel _info;

        public override IExpirable Info
        {
            get
            {
                return _info;
            }
        }

        public virtual void SetDuelPanel(DuelModel model)
        {
            _info = model;
            Name.text = _info.Nickname;
            Bet.text = _info.Bet.ToString();
            LoginManager.Instance.GetUserImage(this, _info.UserId);        
        }

        public void AcceptDuel(bool accept)
        {
            if (accept)
            {
                DuelManager.Instance.AcceptDuelAsync(_info.Id);
                Run();
            }
            else
            {
                DuelManager.Instance.DeclineDuelAsync(_info.Id);
            }
        }

        public void Run()
        {
            Canvaser.Instance.Duels.gameObject.SetActive(false);        
            Canvaser.Instance.StartRun();
        }

        public void SetSprite(Sprite sprite)
        {
            Avatar.sprite = sprite;
        }
    }
}
