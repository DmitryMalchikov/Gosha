using System;

namespace Assets.Scripts.Interfaces
{
    public interface IExpirable
    {
        DateTime ExpireDate { get; set; }
    }
}
