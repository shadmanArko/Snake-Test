using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCode.Persistence
{
    public class JsonDataContext : DataContext
    {
        public override async Task Load()
        {
            if (!File.Exists(SaveDataFilePath))
            {
                return;
            }
            
            using var gameDataFileReader = new StreamReader(SaveDataFilePath);
            var gameDataJson = await gameDataFileReader.ReadToEndAsync();
            JsonUtility.FromJsonOverwrite(gameDataJson, saveData);
        }

        public override async Task Save()
        {
            var json = JsonUtility.ToJson(saveData);
            using var writer = new StreamWriter(SaveDataFilePath);
            await writer.WriteAsync(json);
        }
        
        private string SaveDataFilePath => $"{Application.persistentDataPath}/Saves/save_data.json";
    }
}