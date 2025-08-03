using System;
using System.Collections.Generic;
using _Scripts.EventBus;
using UniRx;
using UnityEngine;
using Zenject;

namespace SnakeSystem
{
    public class SnakeView : MonoBehaviour, ISnakeView, IDisposable
    {
        [SerializeField] private Transform headTransform;
        [SerializeField] private SnakeConfig config;

        // Input observable - View's responsibility to capture input
        private readonly Subject<Direction> _directionInputSubject = new Subject<Direction>();
        public IObservable<Direction> OnDirectionInput => _directionInputSubject;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly List<SnakeBodyPartView> _bodyParts = new List<SnakeBodyPartView>();

        private void Start()
        {
            SetupInputHandling();
        }

        // View-only rendering methods - called by Controller
        public void SetHeadPosition(Vector2Int position)
        {
            headTransform.position = new Vector3(position.x, position.y, 0);
        }

        public void SetHeadRotation(Direction direction)
        {
            var angle = GetAngleFromDirection(direction);
            headTransform.eulerAngles = new Vector3(0, 0, angle - 90);
        }

        public void UpdateBodyParts(IReadOnlyList<SnakeMovePosition> positions)
        {
            // Remove excess body parts
            while (_bodyParts.Count > positions.Count)
            {
                var lastPart = _bodyParts[_bodyParts.Count - 1];
                _bodyParts.RemoveAt(_bodyParts.Count - 1);
                if (lastPart != null)
                {
                    Destroy(lastPart.gameObject);
                }
            }

            // Add new body parts
            while (_bodyParts.Count < positions.Count)
            {
                var bodyPart = CreateBodyPart();
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

        // View handles input capture and notifies through observable
        private void SetupInputHandling()
        {
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.UpArrow))
                .Subscribe(_ => _directionInputSubject.OnNext(Direction.Up))
                .AddTo(_disposables);

            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.DownArrow))
                .Subscribe(_ => _directionInputSubject.OnNext(Direction.Down))
                .AddTo(_disposables);

            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.LeftArrow))
                .Subscribe(_ => _directionInputSubject.OnNext(Direction.Left))
                .AddTo(_disposables);

            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.RightArrow))
                .Subscribe(_ => _directionInputSubject.OnNext(Direction.Right))
                .AddTo(_disposables);
        }

        private SnakeBodyPartView CreateBodyPart()
        {
            var bodyPartGO = new GameObject("SnakeBodyPart");
            var spriteRenderer = bodyPartGO.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = config.snakeBodySprite;
            spriteRenderer.sortingOrder = config.bodyPartSortingOrder;

            return bodyPartGO.AddComponent<SnakeBodyPartView>();
        }

        private float GetAngleFromDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Right => 0f,
                Direction.Up => 90f,
                Direction.Left => 180f,
                Direction.Down => 270f,
                _ => 0f
            };
        }

        public void Dispose()
        {
            _directionInputSubject?.Dispose();
            _disposables?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}