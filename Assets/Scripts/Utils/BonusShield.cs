using Assets.Scripts.DTO;
using Assets.Scripts.Gameplay;

namespace Assets.Scripts.Utils
{
    public class BonusShield : BonusParent
    {
        public BonusShield(Bonus bonus) : base(bonus) { }

        public override bool UseBonus()
        {
            if (Use())
            {
                PlayerShield.Instance.ApplyShield();
                return true;
            }

            return false;
        }
    }
}
