using System;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.DTO
{
    public class DuelModel : IExpirable
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public int Bet { get; set; }
        public int? Result { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
