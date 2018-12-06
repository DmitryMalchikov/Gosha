using System;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.DTO
{
    public class PlayerTaskModel : PlayerTasks, IExpirable
    {
        public int Reward { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
