public interface IBonus
{
    Bonus CurrentBonus { get; }
    bool UseBonus();
}

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
