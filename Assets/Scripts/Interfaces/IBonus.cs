using Assets.Scripts.DTO;

namespace Assets.Scripts.Interfaces
{
    public interface IBonus
    {
        int Count { get; }
        Bonus CurrentBonus { get; }
        bool UseBonus();
    }
}
