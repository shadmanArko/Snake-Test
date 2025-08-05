using _Scripts.Entities.Snake.View;

namespace _Scripts.Entities.Snake.Factory
{
    public interface ISnakeBodyPartFactory
    {
        SnakeBodyPartView CreateBodyPart();
    }
}