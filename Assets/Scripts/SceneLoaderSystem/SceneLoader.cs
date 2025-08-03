using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace SceneLoaderSystem
{
    public class SceneLoader
    {
        public async UniTask LoadSceneAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync("Loading");
            await UniTask.Delay(500);
            await SceneManager.LoadSceneAsync(sceneName);
        }
    }
}