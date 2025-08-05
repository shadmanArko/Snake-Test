using _Scripts.Entities.GameOverAndPause.Config;
using _Scripts.Entities.GameOverAndPause.Controller;
using _Scripts.Entities.GameOverAndPause.Model;
using _Scripts.Entities.GameOverAndPause.View;
using _Scripts.Entities.Food.Config;
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
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSceneInstaller", menuName = "Installers/GameSceneInstaller")]
public class GameSceneInstaller : ScriptableObjectInstaller<GameSceneInstaller>
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