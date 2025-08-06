using System;
using System.Collections.Generic;
using _Scripts.Entities.Snake.ValueObjects;
using UnityEngine;

namespace _Scripts.Entities.Snake.View
{
    public class SnakeView : MonoBehaviour, ISnakeView
    {
        [SerializeField] private Transform headTransform;
        
        private readonly List<SnakeBodyPartView> _bodyParts = new();
        
        public void SetHeadPosition(Vector2Int position)
        {
            headTransform.position = new Vector3(position.x, position.y, 0);
        }

        public void SetHeadRotation(float angle)
        {
            headTransform.eulerAngles = new Vector3(0, 0, angle - 90);
        }

        public void UpdateBodyParts(IReadOnlyList<SnakeMovePosition> positions, Func<SnakeBodyPartView> bodyPartCreator)
        {
            // Remove excess body parts
            while (_bodyParts.Count > positions.Count)
            {
                var lastPart = _bodyParts[^1];
                _bodyParts.RemoveAt(_bodyParts.Count - 1);
                if (lastPart != null)
                {
                    Destroy(lastPart.gameObject);
                }
            }

            // Add new body parts using the provided creator
            while (_bodyParts.Count < positions.Count)
            {
                var bodyPart = bodyPartCreator();
                _bodyParts.Add(bodyPart);
            }

            // Update positions
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