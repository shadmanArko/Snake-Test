using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;

namespace _Scripts.GlobalConfigs
{
    public class RemoteConfigManager : MonoBehaviour
    {
        public static RemoteConfigManager Instance;
        
        [Header("Config Reference")]
        public GameConfig localGameConfig; // Assign your ScriptableObject here
        
        [Header("Remote Config Settings")]
        public bool useRemoteConfig = true;
        public bool debugLogs = true;
        
        private bool firebaseInitialized = false;
        private const string GAME_CONFIG_KEY = "gameConfig";
        
        public event Action OnConfigLoaded;
        public event Action<string> OnConfigError;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeFirebaseAndLoadConfig();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private async void InitializeFirebaseAndLoadConfig()
        {
            try
            {
                var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
                
                if (dependencyStatus == DependencyStatus.Available)
                {
                    firebaseInitialized = true;
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
                    LogError($"Firebase initialization failed: {dependencyStatus}");
                    OnConfigError?.Invoke($"Firebase initialization failed: {dependencyStatus}");
                }
            }
            catch (Exception e)
            {
                LogError($"Exception during Firebase initialization: {e.Message}");
                OnConfigError?.Invoke(e.Message);
            }
        }
        
        private async Task SetupRemoteConfigDefaults()
        {
            try
            {
                // Set default values from local ScriptableObject
                var defaultConfigData = new GameConfigData(localGameConfig);
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
                
                // Check current remote config info
                var info = FirebaseRemoteConfig.DefaultInstance.Info;
                LogDebug($"Remote Config Info - Last Fetch Status: {info.LastFetchStatus},");
                
                // Fetch remote config with shorter cache for testing (5 minutes)
                var fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.FromMinutes(5));
                await fetchTask;
                
                // Get updated info after fetch
                info = FirebaseRemoteConfig.DefaultInstance.Info;
                LogDebug($"After Fetch - Last Fetch Status: {info.LastFetchStatus}");
                
                if (fetchTask.IsCompletedSuccessfully)
                {
                    LogDebug("Remote config fetched successfully");
                    
                    // Check if we have any remote values before activation
                    var testValue = FirebaseRemoteConfig.DefaultInstance.GetValue(GAME_CONFIG_KEY);
                    LogDebug($"Remote config value before activation: {testValue.StringValue}");
                    LogDebug($"Value source: {testValue.Source}");
                    
                    // Activate the fetched config
                    var activateTask = FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                    await activateTask;
                    
                    // Check value after activation attempt (regardless of activation result)
                    var currentValue = FirebaseRemoteConfig.DefaultInstance.GetValue(GAME_CONFIG_KEY);
                    LogDebug($"Current config value: {currentValue.StringValue}");
                    LogDebug($"Current value source: {currentValue.Source}");
                    
                    if (activateTask.Result)
                    {
                        LogDebug("Remote config activated successfully - new data available");
                        ApplyRemoteConfig();
                        OnConfigLoaded?.Invoke();
                    }
                    else
                    {
                        LogDebug($"No new remote config to activate (cached data is current). Fetch Status: {info.LastFetchStatus}");
                        
                        // Even if activation returns false, we might still have valid remote data
                        // Check if we have remote data (not default) and apply it
                        if (currentValue.Source == Firebase.RemoteConfig.ValueSource.RemoteValue || 
                            (currentValue.Source == Firebase.RemoteConfig.ValueSource.DefaultValue && !string.IsNullOrEmpty(currentValue.StringValue)))
                        {
                            LogDebug("Applying existing remote config data");
                            ApplyRemoteConfig();
                        }
                        else
                        {
                            LogWarning("No valid remote config data available, using local config");
                        }
                        
                        OnConfigLoaded?.Invoke();
                    }
                }
                else
                {
                    LogWarning($"Failed to fetch remote config. Status: {info.LastFetchStatus}");
                    OnConfigLoaded?.Invoke();
                }
            }
            catch (Exception e)
            {
                LogError($"Exception during remote config fetch: {e.Message}");
                LogError($"Stack trace: {e.StackTrace}");
                OnConfigError?.Invoke(e.Message);
            }
        }
        
        private void ApplyRemoteConfig()
        {
            try
            {
                var configValue = FirebaseRemoteConfig.DefaultInstance.GetValue(GAME_CONFIG_KEY);
                string remoteConfigJson = configValue.StringValue;
                
                LogDebug($"Applying config from source: {configValue.Source}");
                
                if (!string.IsNullOrEmpty(remoteConfigJson))
                {
                    LogDebug($"Config JSON: {remoteConfigJson}");
                    
                    var remoteConfigData = JsonUtility.FromJson<GameConfigData>(remoteConfigJson);
                    
                    if (remoteConfigData != null)
                    {
                        remoteConfigData.ApplyToScriptableObject(localGameConfig);
                        LogDebug("Remote config applied to ScriptableObject successfully");
                        
                        // Log the applied values for verification
                        LogDebug($"Applied values - Grid: {localGameConfig.gridWidth}x{localGameConfig.gridHeight}, " +
                               $"Snake Speed: {localGameConfig.snakeMoveInterval}, Score: {localGameConfig.scorePerFood}");
                    }
                    else
                    {
                        LogError("Failed to parse remote config JSON");
                    }
                }
                else
                {
                    LogWarning("Remote config JSON is empty, keeping local values");
                }
            }
            catch (Exception e)
            {
                LogError($"Failed to apply remote config: {e.Message}");
                LogError($"Stack trace: {e.StackTrace}");
            }
        }
        
        // Manual refresh method you can call anytime - with force fetch
        public async void RefreshConfig(bool forceRefresh = false)
        {
            if (firebaseInitialized && useRemoteConfig)
            {
                try
                {
                    LogDebug($"Refreshing config (force: {forceRefresh})...");
                    
                    // Use zero cache time to force fetch if needed
                    TimeSpan cacheExpiration = forceRefresh ? TimeSpan.Zero : TimeSpan.FromMinutes(5);
                    
                    await FirebaseRemoteConfig.DefaultInstance.FetchAsync(cacheExpiration);
                    bool activated = await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                    
                    LogDebug($"Config refresh completed. Activated: {activated}");
                    
                    // Apply config regardless of activation result (it might be cached)
                    ApplyRemoteConfig();
                    OnConfigLoaded?.Invoke();
                }
                catch (Exception e)
                {
                    LogError($"Failed to refresh config: {e.Message}");
                }
            }
        }
        
        // Force refresh config (ignores cache)
        public async void ForceRefreshConfig()
        {
            RefreshConfig(true);
        }
        
        // Helper method to get current config (returns your ScriptableObject)
        public GameConfig GetCurrentConfig()
        {
            return localGameConfig;
        }
        
        // Upload current local config to Firebase Console (for initial setup)
        public void LogCurrentConfigForFirebaseConsole()
        {
            var configData = new GameConfigData(localGameConfig);
            string json = JsonUtility.ToJson(configData, true);
            Debug.Log($"Copy this JSON to Firebase Console under key '{GAME_CONFIG_KEY}':\n{json}");
        }
        
        // Test method to check Firebase Remote Config setup
        [ContextMenu("Test Remote Config Connection")]
        public async void TestRemoteConfigConnection()
        {
            if (!firebaseInitialized)
            {
                LogError("Firebase not initialized yet!");
                return;
            }
            
            try
            {
                LogDebug("Testing Remote Config connection...");
                
                // Check current status
                var info = FirebaseRemoteConfig.DefaultInstance.Info;
                LogDebug($"Current Remote Config Status: {info.LastFetchStatus}");
                //LogDebug($"Last Fetch Time: {info.LastFetchTime}");
                
                // Force fetch with no cache
                await FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
                
                // Check all available keys
                var keys = FirebaseRemoteConfig.DefaultInstance.Keys;
                LogDebug($"Available Remote Config Keys: [{string.Join(", ", keys)}]");
                
                // Check our specific key
                var value = FirebaseRemoteConfig.DefaultInstance.GetValue(GAME_CONFIG_KEY);
                LogDebug($"GameConfig Value: {value.StringValue}");
                LogDebug($"GameConfig Source: {value.Source}");
                
                // Try to activate
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