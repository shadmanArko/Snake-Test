using System;
using LevelSystem.Config;
using UnityEngine;
using Zenject;

namespace LevelSystem
{
    public class FoodView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        public void SetFoodPosition(Vector2Int position)
        {
            transform.position = new Vector3(position.x, position.y, 0);
        }
        
        public void ApplyVto(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }

}