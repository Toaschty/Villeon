﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Villeon.GUI;

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

            // Serialize player inventory
            string inventoryData = "[";
            inventoryData += SaveInventory(InventoryMenu.GetInstance().WeaponInventory);
            inventoryData += SaveInventory(InventoryMenu.GetInstance().PotionInventory);
            inventoryData += SaveInventory(InventoryMenu.GetInstance().MaterialInventory);
            inventoryData += "]";

            // Convert to hex
            string saveData = playerStats + "   " + inventoryData;
            byte[] saveDataBytes = Encoding.Default.GetBytes(saveData);
            var hexString = BitConverter.ToString(saveDataBytes);
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

            // Split stat data from inventory data
            string[] data = saveGame.Split("   ");

            // Convert save data to json objects
            dynamic statsJson = JsonConvert.DeserializeObject<dynamic>(data[0]) !;
            List<dynamic> inventoryJson = JsonConvert.DeserializeObject<List<dynamic>>(data[1]) !;

            // Set stats variables
            Stats.GetInstance().Level = statsJson.Level;
            Stats.GetInstance().Experience = statsJson.Experience;
            Stats.GetInstance().RequiredExperience = statsJson.RequiredExperience;
            Stats.GetInstance().HealthLevel = statsJson.HealthLevel;
            Stats.GetInstance().AttackLevel = statsJson.AttackLevel;
            Stats.GetInstance().DefenseLevel = statsJson.DefenseLevel;

            // Fill inventory with items
            for (int i = 0; i < 32; i++)
            {
                // Fill weapon slot
                if (inventoryJson[i].Item != null)
                {
                    InventoryMenu.GetInstance().WeaponInventory[i / 8, i % 8].SetItem(ItemLoader.GetItem((string)inventoryJson[i].Item), (int)inventoryJson[i].Count);
                    InventoryMenu.GetInstance().MaterialInventory[i / 8, i % 8].ReloadEntities();
                }

                // Fill potion slot
                if (inventoryJson[i + 32].Item != null)
                {
                    InventoryMenu.GetInstance().PotionInventory[i / 8, i % 8].SetItem(ItemLoader.GetItem((string)inventoryJson[i + 32].Item), (int)inventoryJson[i + 32].Count);
                    InventoryMenu.GetInstance().PotionInventory[i / 8, i % 8].ReloadEntities();
                }

                // Fill material slot
                if (inventoryJson[i + 64].Item != null)
                {
                    InventoryMenu.GetInstance().MaterialInventory[i / 8, i % 8].SetItem(ItemLoader.GetItem((string)inventoryJson[i + 64].Item), (int)inventoryJson[i + 64].Count);
                    InventoryMenu.GetInstance().MaterialInventory[i / 8, i % 8].ReloadEntities();
                }
            }
        }

        // Return data string of given inventory
        private static string SaveInventory(InventorySlot[,] inv)
        {
            string data = string.Empty;

            foreach (var weapon in inv)
            {
                // Create new save class
                InventorySlotSaveClass saveSlot = new InventorySlotSaveClass();

                saveSlot.Item = (weapon.Item != null) ? weapon.Item!.Name : null;
                saveSlot.Count = weapon.Count;

                // Serialize data
                data += Newtonsoft.Json.JsonConvert.SerializeObject(saveSlot, Formatting.Indented, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                });

                data += ",";
            }

            return data;
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
