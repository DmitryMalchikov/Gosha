using System;
using System.Collections.Generic;

namespace Assets.Scripts.DTO
{
    public class UserInfoModel
    {
        public int Id { get; set; }
        public int IceCream { get; set; }
        public int HighScore { get; set; }
        public string Nickname { get; set; }
        public int IncomingFriendships { get; set; }
        public int IncomingTrades { get; set; }
        public int IncomingDuels { get; set; }
        public int NewFriendships { get; set; }
        public int NewTrades { get; set; }
        public int NewDuels { get; set; }
        public int DaysInRow { get; set; }
        public bool GotDailyBonus { get; set; }
        public int DuelWins { get; set; }
        public string Region { get; set; }
        public bool CanOfferTrade { get; set; }
        public DateTime? BunnedUntil { get; set; }
        public int Cases { get; set; }
        public int CaseId { get; set; }
        public PlayerTasks[] Achievements { get; set; }
        public PlayerTasks[] WeeklyTasks { get; set; }
        public List<BonusUpgrade> BonusUpgrades { get; set; }
        public List<Bonus> Bonuses { get; set; }
        public string ShopHash { get; set; }
        public string DuelsHash { get; set; }
        public string FriendsHash { get; set; }
        public string SuitsHash { get; set; }
        public string TradesHash { get; set; }

        public UserInfoModel()
        {
            BonusUpgrades = new List<BonusUpgrade>();
            Bonuses = new List<Bonus>();
        }
    }
}
