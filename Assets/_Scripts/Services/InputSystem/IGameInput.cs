using _Scripts.Enums;
using UniRx;

namespace _Scripts.Services.InputSystem
{
    public interface IGameInput
    {
        ReactiveProperty<Direction?> DirectionInput { get; }
    }
}