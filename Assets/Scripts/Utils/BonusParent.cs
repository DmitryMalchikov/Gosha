public abstract class BonusParent : IBonus
{
    Bonus _childBonus;

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

    public abstract bool UseBonus();
}
