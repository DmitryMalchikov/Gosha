using System;

namespace Assets.Scripts.DTO
{
    public class WeeklyTaskModel
    {
        public int Id { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public int ActionCount { get; set; }
        public bool InOneRun { get; set; }
        public string Type { get; set; }
        public int TaskId { get; set; }
    }
}
