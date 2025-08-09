using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace _Scripts.Services.Persistence
{
    public class JsonDataContext : DataContext
    {
        private const string SaveFileName = "save_data.json";
        private const string SavesDirectoryName = "Saves";
        
        private string SaveDataFilePath => Path.Combine(Application.persistentDataPath, SavesDirectoryName, SaveFileName);

        public override async Task Load()
        {
            if (!File.Exists(SaveDataFilePath))
            {
                return;
            }
            
            try
            {
                using var gameDataFileReader = new StreamReader(SaveDataFilePath);
                var gameDataJson = await gameDataFileReader.ReadToEndAsync();
                
                if (!string.IsNullOrEmpty(gameDataJson))
                {
                    JsonUtility.FromJsonOverwrite(gameDataJson, saveData);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load save data: {e.Message}");
            }
        }

        public override async Task Save()
        {
            try
            {
                EnsureSaveDirectoryExists();
                
                var json = JsonUtility.ToJson(saveData, true);
                using var writer = new StreamWriter(SaveDataFilePath);
                await writer.WriteAsync(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save data: {e.Message}");
            }
        }

        private void EnsureSaveDirectoryExists()
        {
            var saveDirectory = Path.GetDirectoryName(SaveDataFilePath);
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
        }
    }
}