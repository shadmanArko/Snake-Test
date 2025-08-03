using UnityEngine;
using Zenject;

namespace SnakeSystem
{
    public class SnakeInstaller : MonoInstaller
    {
        [SerializeField] private SnakeConfig snakeConfig;
        [SerializeField] private SnakeView snakeView; // Reference to view in scene

        public override void InstallBindings()
        {
            Container.Bind<SnakeConfig>().FromInstance(snakeConfig).AsSingle();
            Container.Bind<ISnakeModel>().To<SnakeModel>().AsSingle();
            Container.Bind<ISnakeView>().FromInstance(snakeView).AsSingle();
            Container.Bind<ISnakeController>().To<SnakeController>().AsSingle();
        
            // Initialize controller after all bindings
            Container.Resolve<ISnakeController>().Initialize();
        }
    }
}