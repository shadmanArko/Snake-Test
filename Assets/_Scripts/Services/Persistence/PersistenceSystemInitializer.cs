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
        private readonly DataContext _context;

        private readonly Levels _levels;

        private readonly TextAsset _saveDataJsonFile;

        private static string SaveDataFilePath => $"{Application.persistentDataPath}/Saves/save_data.json";
        
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
                Debug.LogError("Unable to load data:" + e);
            }
        }
        private async Task LoadDataAsync()
        {
            Debug.Log(Application.persistentDataPath);
            var rootSaveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
            if (!Directory.Exists(rootSaveDirectory))
            {
                Directory.CreateDirectory(rootSaveDirectory);
            }
            
            if (!File.Exists(SaveDataFilePath))
            {
                await BootstrapSaveData();
            }
            
            await _context.Load();

            BindContexts();
        }

        private void BindContexts()
        {
            _levels.context = _context;
        }

        private async Task BootstrapSaveData()
        {
            await using var writer = new StreamWriter(SaveDataFilePath);
            await writer.WriteAsync(_saveDataJsonFile.text);
        }
    }
}