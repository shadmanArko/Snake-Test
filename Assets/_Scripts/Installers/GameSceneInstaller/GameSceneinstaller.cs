using GameOverAndPauseSystem.Config;
using GameOverAndPauseSystem.Controller;
using GameOverAndPauseSystem.Model;
using GameOverAndPauseSystem.View;
using LevelSystem;
using LevelSystem.Config;
using LevelSystem.Controller;
using LevelSystem.Model;
using ScoreSystem.Config;
using ScoreSystem.Controller;
using ScoreSystem.Model;
using ScoreSystem.View;
using SnakeSystem;
using SnakeSystem.Factory;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSceneinstaller", menuName = "Installers/GameSceneinstaller")]
public class GameSceneinstaller : ScriptableObjectInstaller<GameSceneinstaller>
{
    [SerializeField] private SnakeConfig snakeConfig;
    [SerializeField] private SnakeView snakeView;
    
    [SerializeField] private FoodConfig foodConfig;
    [SerializeField] private FoodView foodView;
    
    [SerializeField] private ScoreConfig scoreConfig;
    [SerializeField] private ScoreView scoreView;
    
    [SerializeField] private GameOverAndPauseConfig gameOverAndPauseConfig;
    [SerializeField] private GameOverAndPauseView gameOverAndPauseView;

    public override void InstallBindings()
    {
        Container.Bind<SnakeConfig>().FromScriptableObject(snakeConfig).AsSingle().NonLazy();
        Container.Bind<ISnakeModel>().To<SnakeModel>().AsSingle().NonLazy();
        Container.Bind<SnakeView>().FromComponentInNewPrefab(snakeView).AsSingle().NonLazy();
        Container.Bind<ISnakeController>().To<SnakeController>().AsSingle().NonLazy();
        Container.Bind<ISnakeBodyPartFactory>().To<SnakeBodyPartFactory>().AsSingle();

        Container.Bind<FoodConfig>().FromScriptableObject(foodConfig).AsSingle().NonLazy();
        Container.Bind<IFoodModel>().To<FoodModel>().AsSingle().NonLazy();
        Container.Bind<FoodView>().FromComponentInNewPrefab(foodView).AsSingle().NonLazy();
        Container.Bind<IFoodController>().To<FoodController>().AsSingle().NonLazy();

        Container.Bind<ScoreConfig>().FromScriptableObject(scoreConfig).AsSingle();
        Container.Bind<IScoreModel>().To<ScoreModel>().AsSingle().NonLazy();
        Container.Bind<IScoreController>().To<ScoreController>().AsSingle().NonLazy();
        Container.Bind<ScoreView>().FromComponentInNewPrefab(scoreView).AsSingle();

        Container.Bind<GameOverAndPauseConfig>().FromScriptableObject(gameOverAndPauseConfig).AsSingle();
        Container.Bind<IGameOverAndPauseModel>().To<GameOverAndPauseModel>().AsSingle();
        Container.Bind<IGameOverAndPauseController>().To<GameOverAndPauseController>().AsSingle().NonLazy();
        Container.Bind<GameOverAndPauseView>().FromComponentInNewPrefab(gameOverAndPauseView).AsSingle();
    }
}