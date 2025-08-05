using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HelperClasses
{
    public static class AddressableHelper
    {
        private static bool _isInitialized = false;

        /// <summary>
        /// Loads a sprite from Addressables using the provided key
        /// </summary>
        /// <param name="spriteKey">The addressable key for the sprite</param>
        /// <param name="updateCatalogs">Whether to check for and update catalogs from CCD (default: true)</param>
        /// <returns>The loaded sprite or null if failed</returns>
        public static async UniTask<Sprite> LoadSpriteAsync(string spriteKey, bool updateCatalogs = true)
        {
            if (string.IsNullOrWhiteSpace(spriteKey))
            {
                Debug.LogError("Sprite key is null or empty!");
                return null;
            }

            try
            {
                // Initialize Addressables if not already done
                if (!_isInitialized)
                {
                    await Addressables.InitializeAsync().ToUniTask();
                    _isInitialized = true;
                }

                // Check for catalog updates if requested
                if (updateCatalogs)
                {
                    await UpdateCatalogsIfNeeded();
                }

                // Load the sprite using the provided key
                var loadHandle = Addressables.LoadAssetAsync<Sprite>(spriteKey);
                var sprite = await loadHandle.ToUniTask();

                if (sprite == null)
                {
                    Debug.LogError($"Loaded sprite is null for key: {spriteKey}");
                    return null;
                }

                return sprite;
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception while loading sprite with key '{spriteKey}': {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Generic method to load any asset type from Addressables
        /// </summary>
        /// <typeparam name="T">The type of asset to load</typeparam>
        /// <param name="assetKey">The addressable key for the asset</param>
        /// <param name="updateCatalogs">Whether to check for and update catalogs from CCD (default: true)</param>
        /// <returns>The loaded asset or null if failed</returns>
        public static async UniTask<T> LoadAssetAsync<T>(string assetKey, bool updateCatalogs = true)
            where T : UnityEngine.Object
        {
            if (string.IsNullOrWhiteSpace(assetKey))
            {
                Debug.LogError($"Asset key is null or empty for type {typeof(T).Name}!");
                return null;
            }

            try
            {
                // Initialize Addressables if not already done
                if (!_isInitialized)
                {
                    await Addressables.InitializeAsync().ToUniTask();
                    _isInitialized = true;
                }

                // Check for catalog updates if requested
                if (updateCatalogs)
                {
                    await UpdateCatalogsIfNeeded();
                }

                // Load the asset using the provided key
                var loadHandle = Addressables.LoadAssetAsync<T>(assetKey);
                var asset = await loadHandle.ToUniTask();

                if (asset == null)
                {
                    Debug.LogError($"Loaded {typeof(T).Name} is null for key: {assetKey}");
                    return null;
                }

                return asset;
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception while loading {typeof(T).Name} with key '{assetKey}': {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks for and updates Addressable catalogs from CCD if updates are available
        /// </summary>
        public static async UniTask UpdateCatalogsIfNeeded()
        {
            try
            {
                var checkHandle = Addressables.CheckForCatalogUpdates();
                var catalogs = await checkHandle.ToUniTask();

                if (catalogs != null && catalogs.Count > 0)
                {
                    var updateHandle = Addressables.UpdateCatalogs(catalogs);
                    await updateHandle.ToUniTask();
                    Debug.Log("Addressables catalogs updated from CCD.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception while updating catalogs: {e.Message}");
            }
        }

        /// <summary>
        /// Forces re-initialization of Addressables on next load
        /// </summary>
        public static void ForceReinitialize()
        {
            _isInitialized = false;
        }

        /// <summary>
        /// Releases a loaded asset to free memory
        /// </summary>
        /// <param name="asset">The asset to release</param>
        public static void ReleaseAsset(UnityEngine.Object asset)
        {
            if (asset != null)
            {
                Addressables.Release(asset);
            }
        }

        /// <summary>
        /// Releases a loaded asset by its handle
        /// </summary>
        /// <param name="handle">The asset handle to release</param>
        public static void ReleaseAsset<T>(
            UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<T> handle)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}