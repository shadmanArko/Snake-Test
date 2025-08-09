using System;

namespace _Scripts.GlobalConfigs
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