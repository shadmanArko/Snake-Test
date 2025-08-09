using System;
using _Scripts.GlobalConfigs;

namespace _Scripts.Services.RemoteConfig
{
    public interface IRemoteConfigManager
    {
        event Action OnConfigLoaded;
        event Action<string> OnConfigError;
        void Initialize();
        void RefreshConfig(bool forceRefresh = false);
        void ForceRefreshConfig();
        GameConfig GetCurrentConfig();
        void LogCurrentConfigForFirebaseConsole();
        void TestRemoteConfigConnection();
    }
}