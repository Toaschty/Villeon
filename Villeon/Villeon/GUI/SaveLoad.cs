using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Villeon.Components;
using Villeon.Helper;

namespace Villeon.GUI
{
    public class SaveLoad
    {
        private static string _saveGamePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.villeon";

        public static void Save()
        {
            // Get / Create .villeon folder in appData
            Directory.CreateDirectory(_saveGamePath);

            // Serialize player stats
            string playerStats = JsonConvert.SerializeObject(Stats.GetInstance());

            // Serialize player inventory
            string inventoryData = "[";
            inventoryData += SaveInventory(InventoryMenu.GetInstance().WeaponInventory);
            inventoryData += SaveInventory(InventoryMenu.GetInstance().PotionInventory);
            inventoryData += SaveInventory(InventoryMenu.GetInstance().MaterialInventory);
            inventoryData += "]";

            // Convert to hex
            string saveData = playerStats + "|||"
                + inventoryData + "|||"
                + StateManager.RayTracingEnabled;
            byte[] saveDataBytes = Encoding.Default.GetBytes(saveData);
            var hexString = BitConverter.ToString(saveDataBytes);
            hexString = hexString.Replace("-", string.Empty);

            // Write data to file
            File.WriteAllText(_saveGamePath + "/savegame", hexString);
        }

        public static void Load()
        {
            // Read save game from disk
            string hexSaveGame;
            try
            {
                hexSaveGame = File.ReadAllText(_saveGamePath + "/savegame");
            }
            catch (Exception)
            {
                Console.WriteLine("No savegame found!");
                return;
            }

            // Decode save data
            string saveGame = DecodeHex(hexSaveGame);

            // Split stat data from inventory data
            string[] data = saveGame.Split("|||");

            // Convert save data to json objects
            dynamic statsJson = JsonConvert.DeserializeObject<dynamic>(data[0]) !;
            List<dynamic> inventoryJson = JsonConvert.DeserializeObject<List<dynamic>>(data[1]) !;
            StateManager.RayTracingEnabled = bool.Parse(data[2]);

            // Set stats variables
            Stats.GetInstance().Level = statsJson.Level;
            Stats.GetInstance().Experience = statsJson.Experience;
            Stats.GetInstance().RequiredExperience = statsJson.RequiredExperience;
            Stats.GetInstance().HealthLevel = statsJson.HealthLevel;
            Stats.GetInstance().AttackLevel = statsJson.AttackLevel;
            Stats.GetInstance().DefenseLevel = statsJson.DefenseLevel;
            Stats.GetInstance().Progress = statsJson.Progress;
            Stats.GetInstance().UnlockProgress = statsJson.UnlockProgress.ToObject<List<int>>();

            // Set equipped weapons
            Item sword = ItemLoader.GetItem(statsJson.ItemDamageName.ToObject<string>());
            Item shield = ItemLoader.GetItem(statsJson.ItemDefenseName.ToObject<string>());
            Stats.GetInstance().SetAttackItem(sword);
            Stats.GetInstance().SetDefenseItem(shield);
            EquipmentMenu.GetInstance().AddAttackWeapon(sword);
            EquipmentMenu.GetInstance().AddDefenseWeapon(shield);

            // Fill inventory with items
            int potionInvIndexStart = Constants.INVENTORY_SLOTS;
            int materialInvIndexStart = 2 * Constants.INVENTORY_SLOTS;
            for (int i = 0; i < Constants.INVENTORY_SLOTS; i++)
            {
                // Fill weapon slot
                if (inventoryJson[i].Item != null)
                {
                    InventoryMenu.GetInstance().WeaponInventory[i / 8, i % 8].SetItem(ItemLoader.GetItem((string)inventoryJson[i].Item), (int)inventoryJson[i].Count);
                    InventoryMenu.GetInstance().WeaponInventory[i / 8, i % 8].ReloadEntities();
                }

                // Fill potion slot
                if (inventoryJson[i + potionInvIndexStart].Item != null)
                {
                    InventoryMenu.GetInstance().PotionInventory[i / 8, i % 8].SetItem(ItemLoader.GetItem((string)inventoryJson[i + potionInvIndexStart].Item), (int)inventoryJson[i + potionInvIndexStart].Count);
                    InventoryMenu.GetInstance().PotionInventory[i / 8, i % 8].ReloadEntities();
                }

                // Fill material slot
                if (inventoryJson[i + materialInvIndexStart].Item != null)
                {
                    InventoryMenu.GetInstance().MaterialInventory[i / 8, i % 8].SetItem(ItemLoader.GetItem((string)inventoryJson[i + materialInvIndexStart].Item), (int)inventoryJson[i + materialInvIndexStart].Count);
                    InventoryMenu.GetInstance().MaterialInventory[i / 8, i % 8].ReloadEntities();
                }
            }
        }

        // Return data string of given inventory
        private static string SaveInventory(InventorySlot[,] inventory)
        {
            string data = string.Empty;

            foreach (var slot in inventory)
            {
                // Create new save class
                InventorySlotSaveClass saveSlot = new InventorySlotSaveClass();

#pragma warning disable CS8601 // Possible null reference assignment.
                saveSlot.Item = slot.Item != null ? slot.Item!.Name : null;
#pragma warning restore CS8601 // Possible null reference assignment.
                saveSlot.Count = slot.Count;

                // Serialize data
                data += JsonConvert.SerializeObject(saveSlot, Formatting.Indented, new JsonSerializerSettings()
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
