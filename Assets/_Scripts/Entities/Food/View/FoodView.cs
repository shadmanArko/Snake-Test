using UnityEngine;

namespace _Scripts.Entities.Food.View
{
    public class FoodView : MonoBehaviour, IFoodView
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        public void SetFoodPosition(Vector2Int position)
        {
            transform.position = new Vector3(position.x, position.y, 0);
        }
        
        public void ApplyVto(Sprite foodSprite)
        {
            _spriteRenderer.sprite = foodSprite;
        }
    }

}