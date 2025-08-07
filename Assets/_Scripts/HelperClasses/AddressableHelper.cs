using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Scripts.HelperClasses
{
    public static class AddressableHelper
    {
        private static bool _isInitialized = false;
        private static readonly Dictionary<string, UnityEngine.Object> _loadedAssets = new Dictionary<string, UnityEngine.Object>();
        private static readonly Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>> _loadingTasks = new Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>>();

        /// <summary>
        /// Loads a sprite from Addressables using the provided key
        /// </summary>
        public static async UniTask<Sprite> LoadSpriteAsync(string spriteKey, bool forceReload = false)
        {
            var result = await LoadAssetAsync<Sprite>(spriteKey, forceReload);
            return result;
        }

        /// <summary>
        /// Generic method to load any asset type from Addressables
        /// </summary>
        public static async UniTask<T> LoadAssetAsync<T>(string assetKey, bool forceReload = false) 
            where T : UnityEngine.Object
        {
            if (string.IsNullOrWhiteSpace(assetKey))
            {
                Debug.LogError($"Asset key is null or empty for type {typeof(T).Name}!");
                return null;
            }

            // Return cached asset if available and not forcing reload
            if (!forceReload && _loadedAssets.TryGetValue(assetKey, out var cachedAsset))
            {
                if (cachedAsset != null && cachedAsset is T typedAsset)
                {
                    Debug.Log($"Using cached asset for key: {assetKey}");
                    return typedAsset;
                }
                else
                {
                    // Remove invalid cached asset
                    _loadedAssets.Remove(assetKey);
                }
            }

            // Check if already loading this asset
            if (_loadingTasks.TryGetValue(assetKey, out var existingTask))
            {
                try
                {
                    Debug.Log($"Waiting for existing load operation for key: {assetKey}");
                    var result = await existingTask.Task;
                    return result as T;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Existing load operation failed for key '{assetKey}': {e.Message}");
                    _loadingTasks.Remove(assetKey);
                }
            }

            // Create new loading task
            var loadingTask = new UniTaskCompletionSource<UnityEngine.Object>();
            _loadingTasks[assetKey] = loadingTask;

            try
            {
                // Initialize if needed
                if (!_isInitialized)
                {
                    try
                    {
                        await Addressables.InitializeAsync().ToUniTask();
                        _isInitialized = true;
                        Debug.Log("Addressables initialized successfully");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to initialize Addressables: {e.Message}");
                        loadingTask.TrySetException(e);
                        _loadingTasks.Remove(assetKey);
                        return null;
                    }
                }

                // Load the asset
                Debug.Log($"Loading asset for key: {assetKey}");
                AsyncOperationHandle<T> handle = default;
                
                try
                {
                    handle = Addressables.LoadAssetAsync<T>(assetKey);
                    var asset = await handle.ToUniTask();

                    if (asset == null)
                    {
                        Debug.LogError($"Loaded asset is null for key: {assetKey}");
                        loadingTask.TrySetResult(null);
                        return null;
                    }

                    // Cache the loaded asset
                    _loadedAssets[assetKey] = asset;
                    
                    Debug.Log($"Successfully loaded and cached asset for key: {assetKey}");
                    loadingTask.TrySetResult(asset);
                    return asset;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load asset with key '{assetKey}': {e.Message}");
                    
                    // Try to release the handle if it was created
                    try
                    {
                        if (handle.IsValid())
                        {
                            Addressables.Release(handle);
                        }
                    }
                    catch (Exception releaseEx)
                    {
                        Debug.LogError($"Failed to release handle for key '{assetKey}': {releaseEx.Message}");
                    }
                    
                    loadingTask.TrySetException(e);
                    return null;
                }
            }
            finally
            {
                // Always remove from loading tasks when done
                _loadingTasks.Remove(assetKey);
            }
        }

        /// <summary>
        /// Releases a cached asset
        /// </summary>
        public static void ReleaseAsset(UnityEngine.Object asset)
        {
            if (asset == null) return;

            try
            {
                // Find and remove from cache
                string keyToRemove = null;
                foreach (var kvp in _loadedAssets)
                {
                    if (kvp.Value == asset)
                    {
                        keyToRemove = kvp.Key;
                        break;
                    }
                }

                if (keyToRemove != null)
                {
                    _loadedAssets.Remove(keyToRemove);
                    Debug.Log($"Removed asset from cache: {keyToRemove}");
                }

                // Release the asset
                Addressables.Release(asset);
                Debug.Log($"Released asset: {asset.name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to release asset '{asset?.name}': {e.Message}");
            }
        }

        /// <summary>
        /// Releases all cached assets
        /// </summary>
        public static void ReleaseAllAssets()
        {
            try
            {
                Debug.Log($"Releasing {_loadedAssets.Count} cached assets");
                
                var assetsToRelease = new List<UnityEngine.Object>(_loadedAssets.Values);
                _loadedAssets.Clear();
                
                foreach (var asset in assetsToRelease)
                {
                    try
                    {
                        if (asset != null)
                        {
                            Addressables.Release(asset);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to release asset '{asset?.name}': {e.Message}");
                    }
                }
                
                Debug.Log("All cached assets released");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to release all assets: {e.Message}");
            }
        }

        /// <summary>
        /// Forces re-initialization on next load
        /// </summary>
        public static void ForceReinitialize()
        {
            _isInitialized = false;
        }

        /// <summary>
        /// Clears all caches
        /// </summary>
        public static void ClearCache()
        {
            _loadedAssets.Clear();
            _loadingTasks.Clear();
        }

        /// <summary>
        /// Gets debug information
        /// </summary>
        public static void LogDebugInfo()
        {
            Debug.Log($"AddressableHelper Debug Info:");
            Debug.Log($"- Initialized: {_isInitialized}");
            Debug.Log($"- Cached Assets: {_loadedAssets.Count}");
            Debug.Log($"- Loading Tasks: {_loadingTasks.Count}");
            
            foreach (var kvp in _loadedAssets)
            {
                Debug.Log($"  Cached: '{kvp.Key}' -> {kvp.Value?.name}");
            }
        }

        /// <summary>
        /// Check for catalog updates manually (call this when you want to update)
        /// </summary>
        public static async UniTask<bool> CheckForUpdatesAsync()
        {
            try
            {
                if (!_isInitialized)
                {
                    await Addressables.InitializeAsync().ToUniTask();
                    _isInitialized = true;
                }

                var checkHandle = Addressables.CheckForCatalogUpdates();
                var catalogsToUpdate = await checkHandle.ToUniTask();

                if (catalogsToUpdate != null && catalogsToUpdate.Count > 0)
                {
                    Debug.Log($"Found {catalogsToUpdate.Count} catalog updates");
                    
                    var updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate);
                    await updateHandle.ToUniTask();
                    
                    Debug.Log("Catalog updates completed");
                    
                    // Clear cache to force reload of updated assets
                    ReleaseAllAssets();
                    ClearCache();
                    
                    return true;
                }
                
                Debug.Log("No catalog updates available");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to check for updates: {e.Message}");
                return false;
            }
        }
    }
}