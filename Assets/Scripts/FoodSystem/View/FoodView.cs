using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LevelSystem
{
    public class FoodView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        public void SetFoodPosition(Vector2Int position)
        {
            transform.position = new Vector3(position.x, position.y, 0);
        }
        
        public async UniTaskVoid ApplyVto(UniTask<Sprite> spriteTask)
        {
            try
            {
                var sprite = await spriteTask;
                if (sprite != null)
                {
                    _spriteRenderer.sprite = sprite;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to apply VTO sprite: {e.Message}");
            }
        }
    }

}