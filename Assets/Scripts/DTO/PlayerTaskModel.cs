using System;

public class PlayerTaskModel : PlayerTasks, IExpirable
{
    public int Reward { get; set; }
    public DateTime ExpireDate { get; set; }
}
