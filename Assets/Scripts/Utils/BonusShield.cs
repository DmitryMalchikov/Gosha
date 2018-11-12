public class BonusShield : BonusParent
{
    public BonusShield(Bonus bonus) : base(bonus) { }

    public override bool UseBonus()
    {
        if (CurrentBonus.Use())
        {
            PlayerShield.Instance.ApplyShield();
            return true;
        }

        return false;
    }
}
