using System;
using System.Collections.Generic;
using _Scripts.Entities.Snake.ValueObjects;
using UnityEngine;

namespace _Scripts.Entities.Snake.View
{
    public interface ISnakeView
    {
        void SetHeadPosition(Vector2Int position);
        void SetHeadRotation(float angle);
        void UpdateBodyParts(IReadOnlyList<SnakeMovePosition> positions, Func<SnakeBodyPartView> bodyPartCreator);
        void ApplyVto(Sprite snakeHeadSprite);
    }
}