public class BonusFreeze : BonusParent
{
    public BonusFreeze(Bonus bonus) : base(bonus) { }

    public override bool UseBonus()
    {
        if (CurrentBonus.Use())
        {
            SpeedController.Instance.ApplyDeceleration();
            return true;
        }

        return false;
    }
}
