using System;

public interface IExpirable
{
    DateTime ExpireDate { get; set; }
}
