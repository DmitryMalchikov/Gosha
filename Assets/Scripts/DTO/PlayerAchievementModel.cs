using System;

namespace Assets.Scripts.DTO
{
    public class PlayerAchievementModel : PlayerTasks
    {
        public string Name { get; set; }
        public int Reward { get; set; }
        public DateTime? CompleteDate { get; set; }
    }
}
