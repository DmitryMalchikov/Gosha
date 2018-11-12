using System.Collections.Generic;

public class DuelsFullInfoModel
{
    public List<DuelModel> DuelOffers { get; set; }
    public List<DuelModel> DuelRequests { get; set; }
    public string DuelsHash { get; set; }
}
