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
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSceneinstaller", menuName = "Installers/GameSceneinstaller")]
public class GameSceneinstaller : ScriptableObjectInstaller<GameSceneinstaller>
{
    [SerializeField] private SnakeConfig snakeConfig;
    [SerializeField] private SnakeView snakeView;
    
    [SerializeField] private GameplayConfig gameplayConfig;
    [SerializeField] private GameplayView gameplayView;
    
    [SerializeField] private ScoreConfig scoreConfig;
    [SerializeField] private ScoreView scoreView;
    
    [SerializeField] private GameOverAndPauseConfig gameOverAndPauseConfig;
    [SerializeField] private GameOverAndPauseView gameOverAndPauseView;

    public override void InstallBindings()
    {
        Container.Bind<SnakeConfig>().FromScriptableObject(snakeConfig).AsSingle().NonLazy();
        Container.Bind<ISnakeModel>().To<SnakeModel>().AsSingle().NonLazy();
        Container.Bind<ISnakeView>().FromComponentInNewPrefab(snakeView).AsSingle().NonLazy();
        Container.Bind<ISnakeController>().To<SnakeController>().AsSingle().NonLazy();

        Container.Bind<GameplayConfig>().FromScriptableObject(gameplayConfig).AsSingle().NonLazy();
        Container.Bind<IGameplayModel>().To<GameplayModel>().AsSingle().NonLazy();
        Container.Bind<GameplayView>().FromComponentInNewPrefab(gameplayView).AsSingle().NonLazy();
        Container.Bind<IGameplayController>().To<GameplayController>().AsSingle().NonLazy();
        Container.Bind<ILevelGrid>().To<LevelGridAdapter>().AsSingle().NonLazy();//todo change name

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