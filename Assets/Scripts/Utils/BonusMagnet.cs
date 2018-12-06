using Assets.Scripts.DTO;
using Assets.Scripts.Gameplay;

namespace Assets.Scripts.Utils
{
    public class BonusMagnet : BonusParent
    {
        public BonusMagnet(Bonus bonus) : base(bonus) { }

        public override bool UseBonus()
        {
            if (Use())
            {
                Collector.Instance.UseMagnet();
                return true;
            }

            return false;
        }
    }
}
