using System;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using _Scripts.Services.SceneFlowManagementSystem.Config;
using UniRx;
using Zenject;

namespace _Scripts.Services.SceneFlowManagementSystem.SceneFlowManager
{
    public class SceneFlowManager : IDisposable, ISceneFlowManager, IInitializable
    {
        private const string BootstrapScene = "Bootstrap";
        
        private readonly ISceneConfig _config;
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        private AsyncOperationHandle<SceneInstance> _mainMenuHandle;
        private AsyncOperationHandle<SceneInstance> _loadingHandle;
        private AsyncOperationHandle<SceneInstance> _gameHandle;

        public SceneFlowManager(ISceneConfig config, IEventBus eventBus)
        {
            _config = config;
            _eventBus = eventBus;
            
            _eventBus.OnEvent<LoadGameSceneEvent>()
                .Subscribe(_ => LoadGameScene()).AddTo(_disposables);
            
            _eventBus.OnEvent<RestartGameSceneEvent>()
                .Subscribe(_ => RestartGameScene()).AddTo(_disposables);
        }

        public void Initialize()
        {
            LoadMainMenuScene();
        }

        private async void LoadMainMenuScene()
        {
            _mainMenuHandle = await LoadSceneAsync(_config.MainMenuScene);
            await SceneManager.UnloadSceneAsync(BootstrapScene);
        }

        public async void LoadGameScene()
        {
            _loadingHandle = await LoadSceneAsync(_config.LoadingScene);
            
            _gameHandle = await LoadSceneAsync(_config.GameScene);
            
            await UnloadSceneAsync(_mainMenuHandle);
            await UnloadSceneAsync(_loadingHandle);
        }

        public async void RestartGameScene()
        {
            var dummyHandle = await Addressables.LoadSceneAsync(BootstrapScene, LoadSceneMode.Additive).ToUniTask();
            SceneManager.SetActiveScene(dummyHandle.Scene);
            
            await UnloadSceneAsync(_gameHandle);

            if (_gameHandle.IsValid())
            {
                Addressables.Release(_gameHandle);
                _gameHandle = default;
            }

            await UniTask.Yield();
            
            _gameHandle = await LoadSceneAsync(_config.GameScene);
            SceneManager.SetActiveScene(_gameHandle.Result.Scene);
            
            await Addressables.UnloadSceneAsync(dummyHandle, true).ToUniTask();
        }

        private async UniTask<AsyncOperationHandle<SceneInstance>> LoadSceneAsync(AssetReference sceneRef)
        {
            var handle = sceneRef.LoadSceneAsync(LoadSceneMode.Additive);
            await handle.ToUniTask();
            return handle;
        }

        private async UniTask UnloadSceneAsync(AsyncOperationHandle<SceneInstance> handle)
        {
            if (handle.IsValid())
            {
                await Addressables.UnloadSceneAsync(handle, true).ToUniTask();
            }
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}