public class BonusMagnet : BonusParent
{
    public BonusMagnet(Bonus bonus) : base(bonus) { }

    public override bool UseBonus()
    {
        if (CurrentBonus.Use())
        {
            Collector.Instance.UseMagnet();
            return true;
        }

        return false;
    }
}
