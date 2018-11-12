using System.Collections.Generic;

public class RegisterExternalBindingModel
{
    public RegisterExternalBindingModel(RegisterBindingModel model)
    {
        Email = model.Email;
        Nickname = model.Nickname;
        PhoneNumber = model.PhoneNumber;
        RegionId = model.RegionId;
    }
    public string Email { get; set; }
    public string Nickname { get; set; }
    public string PhoneNumber { get; set; }
    public int RegionId { get; set; }
    public int IceCream { get; set; }
    public int Cases { get; set; }
    public List<Bonus> Bonuses { get; set; }
    public List<BonusUpgrade> BonusUpgrades { get; set; }
}
