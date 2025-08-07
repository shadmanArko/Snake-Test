using _Scripts.Entities.Snake.View;
using UnityEngine;

namespace _Scripts.Entities.Snake.Factory
{
    public interface ISnakeBodyPartFactory
    {
        SnakeBodyPartView CreateBodyPart(Sprite snakeBodySprite);
    }
}