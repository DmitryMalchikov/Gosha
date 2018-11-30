using Newtonsoft.Json;

public class Bonus
{
    public string Type { get; set; }
    public int Amount { get; set; }
    public int Id { get; set; }
    public NameLocalization Name { get; set; }

    public bool Use()
    {
        if (Amount < 1)
        {
            return false;
        }

        Amount -= 1;
        if (!LoginManager.LocalUser)
        {
            ScoreManager.Instance.UseBonusAsync(Id);
        }
        else
        {
            Canvaser.Instance.SBonuses.SetStartBonuses(LoginManager.User.Bonuses);
            FileExtensions.SaveJsonDataAsync(DataType.UserInfo, LoginManager.User);
        }
        return true;
    }

    public void Copy(Bonus bonus)
    {
        Type = bonus.Type;
        Amount = bonus.Amount;
        Id = bonus.Id;
        Name = bonus.Name;
    }
}
