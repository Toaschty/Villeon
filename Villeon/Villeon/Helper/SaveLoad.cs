using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Villeon.Helper
{
    public class SaveLoad
    {
        private static string _saveGamePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.villeon";

        public static void Save()
        {
            // Get / Create .villeon folder in appData
            Directory.CreateDirectory(_saveGamePath);

            // Serialize player stats
            string playerStats = Newtonsoft.Json.JsonConvert.SerializeObject(Stats.GetInstance());
            byte[] playerStatsBytes = Encoding.Default.GetBytes(playerStats);
            var hexString = BitConverter.ToString(playerStatsBytes);
            hexString = hexString.Replace("-", string.Empty);

            // Write data to file
            File.WriteAllText(_saveGamePath + "/savegame", hexString);
        }

        public static void Load()
        {
            // Read save game from disk
            string hexSaveGame = File.ReadAllText(_saveGamePath + "/savegame");

            // Decode save data
            string saveGame = DecodeHex(hexSaveGame);

            // Convert save data to json object
            dynamic saveGameJson = JsonConvert.DeserializeObject<Stats>(saveGame) !;

            // Set stat variables
            Stats.GetInstance().Level = saveGameJson.Level;
            Stats.GetInstance().Experience = saveGameJson.Experience;
            Stats.GetInstance().RequiredExperience = saveGameJson.RequiredExperience;
            Stats.GetInstance().HealthLevel = saveGameJson.HealthLevel;
            Stats.GetInstance().AttackLevel = saveGameJson.AttackLevel;
            Stats.GetInstance().DefenseLevel = saveGameJson.DefenseLevel;
        }

        // Decode hex string to string
        private static string DecodeHex(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.UTF8.GetString(bytes);
        }
    }
}
