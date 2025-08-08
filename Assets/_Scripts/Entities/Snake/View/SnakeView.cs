using System;
using System.Collections.Generic;
using _Scripts.Entities.Snake.ValueObjects;
using UnityEngine;

namespace _Scripts.Entities.Snake.View
{
    public class SnakeView : MonoBehaviour, ISnakeView
    {
        private const float HeadRotationOffset = -90f;
        
        [SerializeField] private Transform headTransform;
        [SerializeField] private SpriteRenderer headSpriteRenderer;
        
        private readonly List<SnakeBodyPartView> _bodyParts = new();
        
        public void SetHeadPosition(Vector2Int position)
        {
            headTransform.position = new Vector3(position.x, position.y, 0);
        }

        public void SetHeadRotation(float angle)
        {
            headTransform.eulerAngles = new Vector3(0, 0, angle + HeadRotationOffset);
        }

        public void UpdateBodyParts(IReadOnlyList<SnakeMovePosition> positions, Func<SnakeBodyPartView> bodyPartCreator)
        {
            AdjustBodyPartCount(positions.Count, bodyPartCreator);
            UpdateBodyPartPositions(positions);
        }

        public void ApplyVto(Sprite snakeHeadSprite)
        {
            headSpriteRenderer.sprite = snakeHeadSprite;
        }

        private void AdjustBodyPartCount(int targetCount, Func<SnakeBodyPartView> bodyPartCreator)
        {
            RemoveExcessBodyParts(targetCount);
            AddRequiredBodyParts(targetCount, bodyPartCreator);
        }

        private void RemoveExcessBodyParts(int targetCount)
        {
            while (_bodyParts.Count > targetCount)
            {
                var lastPart = _bodyParts[^1];
                _bodyParts.RemoveAt(_bodyParts.Count - 1);
                
                if (lastPart != null)
                {
                    Destroy(lastPart.gameObject);
                }
            }
        }

        private void AddRequiredBodyParts(int targetCount, Func<SnakeBodyPartView> bodyPartCreator)
        {
            while (_bodyParts.Count < targetCount)
            {
                var bodyPart = bodyPartCreator();
                _bodyParts.Add(bodyPart);
            }
        }

        private void UpdateBodyPartPositions(IReadOnlyList<SnakeMovePosition> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if (i < _bodyParts.Count && _bodyParts[i] != null)
                {
                    _bodyParts[i].UpdatePosition(positions[i]);
                }
            }
        }
    }
}