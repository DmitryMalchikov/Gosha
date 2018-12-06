using Assets.Scripts.DTO;

namespace Assets.Scripts.Utils
{
    public class BonusFreeze : BonusParent
    {
        public BonusFreeze(Bonus bonus) : base(bonus) { }

        public override bool UseBonus()
        {
            if (Use())
            {
                SpeedController.Instance.ApplyDeceleration();
                return true;
            }

            return false;
        }
    }
}
