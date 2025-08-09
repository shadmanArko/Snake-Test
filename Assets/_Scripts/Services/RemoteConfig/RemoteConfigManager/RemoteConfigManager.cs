using UnityEngine;
using Firebase;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;
using _Scripts.GlobalConfigs;
using Zenject;

namespace _Scripts.Services.RemoteConfig
{
    public class RemoteConfigManager : IInitializable, IRemoteConfigManager
    {
        private const string GAME_CONFIG_KEY = "gameConfig";
        private const float DEFAULT_CACHE_EXPIRATION_MINUTES = 5f;
        private const float TESTING_CACHE_EXPIRATION_MINUTES = 0.5f;
        
        private readonly GameConfig _localGameConfig;
        
        public bool useRemoteConfig = true;
        public bool debugLogs = true;
        
        private bool _firebaseInitialized = false;
        
        public event Action OnConfigLoaded;
        public event Action<string> OnConfigError;

        public RemoteConfigManager(GameConfig localGameConfig)
        {
            _localGameConfig = localGameConfig;
        }
        
        public void Initialize()
        {
            InitializeFirebaseAndLoadConfig();
        }

        public async void RefreshConfig(bool forceRefresh = false)
        {
            if (!_firebaseInitialized || !useRemoteConfig) return;

            try
            {
                LogDebug($"Refreshing config (force: {forceRefresh})...");
                
                TimeSpan cacheExpiration = forceRefresh ? TimeSpan.Zero : TimeSpan.FromMinutes(DEFAULT_CACHE_EXPIRATION_MINUTES);
                
                await FirebaseRemoteConfig.DefaultInstance.FetchAsync(cacheExpiration);
                bool activated = await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                
                LogDebug($"Config refresh completed. Activated: {activated}");
                
                ApplyRemoteConfig();
                OnConfigLoaded?.Invoke();
            }
            catch (Exception e)
            {
                LogError($"Failed to refresh config: {e.Message}");
            }
        }
        
        public void ForceRefreshConfig()
        {
            RefreshConfig(true);
        }
        
        public GameConfig GetCurrentConfig()
        {
            return _localGameConfig;
        }
        
        public void LogCurrentConfigForFirebaseConsole()
        {
            var configData = new GameConfigData(_localGameConfig);
            string json = JsonUtility.ToJson(configData, true);
            Debug.Log($"Copy this JSON to Firebase Console under key '{GAME_CONFIG_KEY}':\n{json}");
        }
        
        public async void TestRemoteConfigConnection()
        {
            if (!_firebaseInitialized)
            {
                LogError("Firebase not initialized yet!");
                return;
            }
            
            try
            {
                LogDebug("Testing Remote Config connection...");
                
                var info = FirebaseRemoteConfig.DefaultInstance.Info;
                LogDebug($"Current Remote Config Status: {info.LastFetchStatus}");
                
                await FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
                
                var keys = FirebaseRemoteConfig.DefaultInstance.Keys;
                LogDebug($"Available Remote Config Keys: [{string.Join(", ", keys)}]");
                
                var value = FirebaseRemoteConfig.DefaultInstance.GetValue(GAME_CONFIG_KEY);
                LogDebug($"GameConfig Value: {value.StringValue}");
                LogDebug($"GameConfig Source: {value.Source}");
                
                bool activated = await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                LogDebug($"Activation result: {activated}");
                
                if (activated)
                {
                    var activatedValue = FirebaseRemoteConfig.DefaultInstance.GetValue(GAME_CONFIG_KEY);
                    LogDebug($"Activated GameConfig Value: {activatedValue.StringValue}");
                }
            }
            catch (Exception e)
            {
                LogError($"Test failed: {e.Message}");
            }
        }

        private async void InitializeFirebaseAndLoadConfig()
        {
            try
            {
                var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
                
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _firebaseInitialized = true;
                    LogDebug("Firebase initialized successfully");
                    
                    if (useRemoteConfig)
                    {
                        await SetupRemoteConfigDefaults();
                        await FetchAndActivateRemoteConfig();
                    }
                    else
                    {
                        LogDebug("Using local config only");
                        OnConfigLoaded?.Invoke();
                    }
                }
                else
                {
                    HandleFirebaseInitializationError(dependencyStatus.ToString());
                }
            }
            catch (Exception e)
            {
                HandleFirebaseInitializationError(e.Message);
            }
        }

        private void HandleFirebaseInitializationError(string errorMessage)
        {
            LogError($"Firebase initialization failed: {errorMessage}");
            OnConfigError?.Invoke($"Firebase initialization failed: {errorMessage}");
        }
        
        private async Task SetupRemoteConfigDefaults()
        {
            try
            {
                var defaultConfigData = new GameConfigData(_localGameConfig);
                string defaultJson = JsonUtility.ToJson(defaultConfigData, true);
                
                var defaults = new System.Collections.Generic.Dictionary<string, object>
                {
                    { GAME_CONFIG_KEY, defaultJson }
                };
                
                await FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
                LogDebug("Remote config defaults set successfully");
            }
            catch (Exception e)
            {
                LogError($"Failed to set defaults: {e.Message}");
                throw;
            }
        }
        
        private async Task FetchAndActivateRemoteConfig()
        {
            try
            {
                LogDebug("Fetching remote config...");
                
                var info = FirebaseRemoteConfig.DefaultInstance.Info;
                LogDebug($"Remote Config Info - Last Fetch Status: {info.LastFetchStatus}");
                
                var fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.FromMinutes(TESTING_CACHE_EXPIRATION_MINUTES));
                await fetchTask;
                
                info = FirebaseRemoteConfig.DefaultInstance.Info;
                LogDebug($"After Fetch - Last Fetch Status: {info.LastFetchStatus}");
                
                if (fetchTask.IsCompletedSuccessfully)
                {
                    await HandleSuccessfulFetch();
                }
                else
                {
                    HandleFailedFetch(info);
                }
            }
            catch (Exception e)
            {
                LogError($"Exception during remote config fetch: {e.Message}");
                LogError($"Stack trace: {e.StackTrace}");
                OnConfigError?.Invoke(e.Message);
            }
        }

        private async Task HandleSuccessfulFetch()
        {
            LogDebug("Remote config fetched successfully");
            
            var testValue = FirebaseRemoteConfig.DefaultInstance.GetValue(GAME_CONFIG_KEY);
            LogDebug($"Remote config value before activation: {testValue.StringValue}");
            LogDebug($"Value source: {testValue.Source}");
            
            var activateTask = FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
            await activateTask;
            
            var currentValue = FirebaseRemoteConfig.DefaultInstance.GetValue(GAME_CONFIG_KEY);
            LogDebug($"Current config value: {currentValue.StringValue}");
            LogDebug($"Current value source: {currentValue.Source}");
            
            if (activateTask.Result)
            {
                LogDebug("Remote config activated successfully - new data available");
                ApplyRemoteConfig();
            }
            else
            {
                HandleNoNewConfigToActivate(currentValue);
            }
            
            OnConfigLoaded?.Invoke();
        }

        private void HandleNoNewConfigToActivate(ConfigValue currentValue)
        {
            LogDebug("No new remote config to activate (cached data is current)");
            
            if (IsValidRemoteConfigValue(currentValue))
            {
                LogDebug("Applying existing remote config data");
                ApplyRemoteConfig();
            }
            else
            {
                LogWarning("No valid remote config data available, using local config");
            }
        }

        private bool IsValidRemoteConfigValue(ConfigValue value)
        {
            return value.Source == ValueSource.RemoteValue ||
                   (value.Source == ValueSource.DefaultValue && !string.IsNullOrEmpty(value.StringValue));
        }

        private void HandleFailedFetch(ConfigInfo info)
        {
            LogWarning($"Failed to fetch remote config. Status: {info.LastFetchStatus}");
            OnConfigLoaded?.Invoke();
        }
        
        private void ApplyRemoteConfig()
        {
            try
            {
                var configValue = FirebaseRemoteConfig.DefaultInstance.GetValue(GAME_CONFIG_KEY);
                string remoteConfigJson = configValue.StringValue;
                
                LogDebug($"Applying config from source: {configValue.Source}");
                
                if (string.IsNullOrEmpty(remoteConfigJson))
                {
                    LogWarning("Remote config JSON is empty, keeping local values");
                    return;
                }

                LogDebug($"Config JSON: {remoteConfigJson}");
                
                var remoteConfigData = JsonUtility.FromJson<GameConfigData>(remoteConfigJson);
                
                if (remoteConfigData != null)
                {
                    remoteConfigData.ApplyToScriptableObject(_localGameConfig);
                    LogDebug("Remote config applied to ScriptableObject successfully");
                    LogAppliedConfigValues();
                }
                else
                {
                    LogError("Failed to parse remote config JSON");
                }
            }
            catch (Exception e)
            {
                LogError($"Failed to apply remote config: {e.Message}");
                LogError($"Stack trace: {e.StackTrace}");
            }
        }

        private void LogAppliedConfigValues()
        {
            LogDebug($"Applied values - Grid: {_localGameConfig.gridWidth}x{_localGameConfig.gridHeight}, " +
                   $"Snake Speed: {_localGameConfig.snakeMoveInterval}, Score: {_localGameConfig.scorePerFood}");
        }
        
        private void LogDebug(string message)
        {
            if (debugLogs) Debug.Log($"[RemoteConfigManager] {message}");
        }
        
        private void LogWarning(string message)
        {
            if (debugLogs) Debug.LogWarning($"[RemoteConfigManager] {message}");
        }
        
        private void LogError(string message)
        {
            Debug.LogError($"[RemoteConfigManager] {message}");
        }
    }
}