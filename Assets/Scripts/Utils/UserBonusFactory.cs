public class UserBonusFactory
{
    public static IBonus CreateUserBonus(Bonus bonus)
    {
        if (bonus == null)
        {
            return null;
        }

        IBonus result = null;

        switch (bonus.Name.Name)
        {
            case "Shield":
                result = new BonusShield(bonus);
                break;
            case "Magnet":
                result = new BonusMagnet(bonus);
                break;
            case "Freeze":
                result = new BonusFreeze(bonus);
                break;
        }

        return result;
    }
}
