using System;
using System.Collections.Generic;

public class TournamentModel : IExpirable
{
    public int Id { get; set; }
    public DateTime BeginDate { get; set; }
    public string Name { get; set; }
    public string Prizes { get; set; }
    public bool AvaliableWeeklyTasks { get; set; }
    public List<FriendOfferStatisticsModel> TournamentLeaders { get; set; }
    public DateTime ExpireDate { get; set; }
}
