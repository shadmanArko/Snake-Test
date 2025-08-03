using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeSystem
{
    public interface ISnakeView
    {
        void SetHeadPosition(Vector2Int position);
        void SetHeadRotation(Direction direction);
        void UpdateBodyParts(IReadOnlyList<SnakeMovePosition> positions);
        IObservable<Direction> OnDirectionInput { get; }
    }
}