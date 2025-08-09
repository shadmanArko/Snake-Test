using System;
using System.IO;
using System.Threading.Tasks;
using _Scripts.Services.Persistence.Repositories;
using UnityEngine;
using Zenject;

namespace _Scripts.Services.Persistence
{
    public class PersistenceSystemInitializer : IInitializable, IPersistenceSystemInitializer
    {
        private const string SavesDirectoryName = "Saves";
        private const string SaveFileName = "save_data.json";
        
        private readonly DataContext _context;
        private readonly Levels _levels;
        private readonly TextAsset _saveDataJsonFile;
        
        private static string SaveDataFilePath => Path.Combine(Application.persistentDataPath, SavesDirectoryName, SaveFileName);
        
        public PersistenceSystemInitializer(DataContext context, Levels levels, TextAsset saveDataJsonFile)
        {
            _context = context;
            _levels = levels;
            _saveDataJsonFile = saveDataJsonFile;
        }
        
        public async void Initialize()
        {
            try
            {
                await LoadDataAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to load data: {e.Message}");
                Debug.LogException(e);
            }
        }

        private async Task LoadDataAsync()
        {
            Debug.Log($"Persistent data path: {Application.persistentDataPath}");
            
            EnsureSaveDirectoryExists();
            await EnsureSaveFileExists();
            await LoadContextData();
            BindContexts();
        }

        private void EnsureSaveDirectoryExists()
        {
            var saveDirectory = Path.Combine(Application.persistentDataPath, SavesDirectoryName);
            
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
                Debug.Log($"Created save directory: {saveDirectory}");
            }
        }

        private async Task EnsureSaveFileExists()
        {
            if (!File.Exists(SaveDataFilePath))
            {
                await BootstrapSaveData();
            }
        }

        private async Task LoadContextData()
        {
            await _context.Load();
        }

        private void BindContexts()
        {
            _levels.context = _context;
        }

        private async Task BootstrapSaveData()
        {
            try
            {
                await using var writer = new StreamWriter(SaveDataFilePath);
                await writer.WriteAsync(_saveDataJsonFile.text);
                Debug.Log($"Bootstrapped save data from template: {SaveDataFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to bootstrap save data: {e.Message}");
                throw;
            }
        }
    }
}