using Assets.Scripts.DTO;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.UI;

namespace Assets.Scripts.Utils
{
    public class BonusParent : IBonus
    {
        Bonus _childBonus;

        public int Count { get { return _childBonus == null ? 0 : _childBonus.Amount; } }

        public BonusParent() { }

        public BonusParent(Bonus bonus)
        {
            _childBonus = bonus;
        }

        public Bonus CurrentBonus
        {
            get
            {
                return _childBonus;
            }
        }

        public bool Use()
        {
            if (_childBonus.Amount < 1)
            {
                return false;
            }

            _childBonus.Amount -= 1;
            if (!LoginManager.LocalUser)
            {
                ScoreManager.Instance.UseBonusAsync(_childBonus.Id);
            }
            else
            {
                Canvaser.Instance.SBonuses.SetStartBonuses(LoginManager.User.Bonuses);
                FileExtensions.SaveJsonDataAsync(DataType.UserInfo, LoginManager.User);
            }
            return true;
        }

        public virtual bool UseBonus() { return false; }
    }
}
