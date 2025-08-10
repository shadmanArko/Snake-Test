using System;
using _Scripts.Entities.GameOverAndPause.Config;
using _Scripts.Entities.GameOverAndPause.Controller;
using _Scripts.Entities.GameOverAndPause.Model;
using _Scripts.Entities.GameOverAndPause.View;
using _Scripts.Entities.Food.Controller;
using _Scripts.Entities.Food.Model;
using _Scripts.Entities.Food.View;
using _Scripts.Entities.Score.Config;
using _Scripts.Entities.Score.Controller;
using _Scripts.Entities.Score.Model;
using _Scripts.Entities.Score.View;
using _Scripts.Entities.Snake.Config;
using _Scripts.Entities.Snake.Controller;
using _Scripts.Entities.Snake.Factory;
using _Scripts.Entities.Snake.Model;
using _Scripts.Entities.Snake.View;
using _Scripts.HelperClasses;
using _Scripts.Services.InputSystem;
using _Scripts.Services.Persistence;
using UniRx;
using UnityEngine;
using Zenject;


public class GameSceneInit : IInitializable, IDisposable
{
    [Inject] private CompositeDisposable _disposables;
    [Inject] private IUnitOfWork _unitOfWork;
    public void Dispose()
    {
        _unitOfWork.Save();
        _disposables?.Dispose();
        AddressableHelper.ReleaseAllAssets();
    }

    public void Initialize()
    {
        
    }
}

[CreateAssetMenu(fileName = "GameSceneInstaller", menuName = "Installers/GameSceneInstaller")]
public class GameSceneInstaller : ScriptableObjectInstaller<GameSceneInstaller>
{
    [SerializeField] private SnakeConfig snakeConfig;
    [SerializeField] private SnakeView snakeView;
    
    [SerializeField] private FoodView foodView;
    
    [SerializeField] private ScoreConfig scoreConfig;
    [SerializeField] private ScoreView scoreView;
    
    [SerializeField] private GameOverAndPauseConfig gameOverAndPauseConfig;
    [SerializeField] private GameOverAndPauseView gameOverAndPauseView;

    public override void InstallBindings()
    {
        Container.Bind<CompositeDisposable>().AsSingle();
        Container.BindInterfacesTo<GameSceneInit>().AsSingle().NonLazy();
        
        Container.BindInterfacesTo<SnakeController>().AsSingle();
        Container.BindInterfacesTo<SnakeModel>().AsSingle();
        Container.BindInterfacesTo<SnakeView>().FromComponentInNewPrefab(snakeView).AsSingle();
        Container.BindInterfacesTo<SnakeConfig>().FromScriptableObject(snakeConfig).AsSingle();
        Container.BindInterfacesTo<SnakeBodyPartFactory>().AsSingle();
        
        Container.BindInterfacesTo<FoodController>().AsSingle();
        Container.BindInterfacesTo<FoodModel>().AsSingle();
        Container.BindInterfacesTo<FoodView>().FromComponentInNewPrefab(foodView).AsSingle();
        
        Container.BindInterfacesTo<ScoreController>().AsSingle();
        Container.BindInterfacesTo<ScoreModel>().AsSingle();
        Container.BindInterfacesTo<ScoreView>().FromComponentInNewPrefab(scoreView).AsSingle();
        Container.BindInterfacesTo<ScoreConfig>().FromScriptableObject(scoreConfig).AsSingle();

        Container.BindInterfacesTo<GameOverAndPauseController>().AsSingle();
        Container.BindInterfacesTo<GameOverAndPauseModel>().AsSingle();
        Container.BindInterfacesTo<GameOverAndPauseView>().FromComponentInNewPrefab(gameOverAndPauseView).AsSingle();
        Container.BindInterfacesTo<GameOverAndPauseConfig>().FromScriptableObject(gameOverAndPauseConfig).AsSingle();
        
    #if UNITY_EDITOR || UNITY_STANDALONE
        // PC Input for Editor and Standalone builds
        Container.BindInterfacesTo<PCInput>().AsSingle();
        Debug.Log("PC Input System Installed");
        
    #elif UNITY_ANDROID || UNITY_IOS
        // Mobile Input for Android and iOS builds
        Container.BindInterfacesTo<MobileTouchInput>().AsSingle();
        Debug.Log("Mobile Touch Input System Installed");
        
    #else
        // Fallback to PC input for other platforms
        Container.BindInterfacesTo<PCInput>().AsSingle();
        Debug.Log("Fallback PC Input System Installed");
    #endif
    }
}