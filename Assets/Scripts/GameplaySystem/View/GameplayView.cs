using System;
using LevelSystem.Config;
using UnityEngine;
using Zenject;

namespace LevelSystem
{
    public class GameplayView : MonoBehaviour, IDisposable
    {
        [SerializeField] private GameplayConfig _config;
    
        private GameObject _foodGameObject;
        public void SetFoodPosition(Vector2Int position)
        {
            if (_foodGameObject == null)
            {
                CreateFoodGameObject();
            }
        
            _foodGameObject.transform.position = new Vector3(position.x, position.y, 0);
        }
        

        private void CreateFoodGameObject()
        {
            _foodGameObject = new GameObject("Food");
            var spriteRenderer = _foodGameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _config.foodSprite;
            spriteRenderer.sortingOrder = _config.foodSortingOrder;
        }

        public void Dispose()
        {
            if (_foodGameObject != null)
            {
                Destroy(_foodGameObject);
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }

}