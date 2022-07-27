using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Zenseless.OpenTK;

namespace Villeon.Generation
{
    public static class NPCLoader
    {
        private static List<string> _spawnedNPCs = new List<string>();

        public static void SpawnUnlockableNPCs()
        {
            // Load the NPC Unlock JSON
            dynamic npcUnlocks = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.NPCUnlocks.json")) !;

            // Get cave count
            int caveCount = npcUnlocks.Count;
            for (int i = 0; i < caveCount; i++)
            {
                // Get count of unlockable npc in this cave
                int caveNpcCount = npcUnlocks[i].unlocks.Count;

                // Get unlock progress of player
                int unlockProgress = Stats.GetInstance().GetUnlockProgress(i);

                // Depending on progress -> Load and Spawn in NPCs
                for (int j = 0; j < (unlockProgress > caveNpcCount ? caveNpcCount : unlockProgress); j++)
                {
                    // Select npc to spawn
                    string sceneName = npcUnlocks[i].unlocks[j].scene;
                    string npcName = npcUnlocks[i].unlocks[j].npc;

                    if (_spawnedNPCs.Contains(npcName))
                        continue;

                    // Get npc data from json
                    dynamic npc = GetNPC(sceneName, npcName);

                    // Load npc data and spawn npc
                    LoadNPCData(sceneName, npc, true);

                    _spawnedNPCs.Add(npcName);
                }
            }
        }

        public static void LoadNpcs(string sceneName)
        {
            // Load the JSON
            JObject npcs = (JObject)JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.NPCs.json")) !;

            // Choose the current scene
            dynamic scene = npcs.SelectToken(sceneName) !;

            // Add all the NPCs to the Scene
            int npcCount = scene.Count;
            for (int i = 0; i < npcCount; i++)
            {
                LoadNPCData(sceneName, scene[i], false);
            }
        }

        public static dynamic GetNPC(string sceneName, string npcName)
        {
            // Load the JSON
            JObject npcs = (JObject)JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.NPCs.json")) !;

            dynamic scene = npcs.SelectToken(sceneName) !;

            // Find npc with name
            int npcCount = scene.Count;
            for (int i = 0; i < npcCount; i++)
            {
                if (scene[i].name == npcName)
                {
                    // Return found npc
                    return scene[i];
                }
            }

            // Not found -> Return first npc
            return scene[0];
        }

        public static void SpawnRescuedNPC(string sceneName, int caveIndex)
        {
            dynamic npcUnlocks = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.NPCUnlocks.json")) !;

            // Use this to find the right npc
            int unlockProgress = Stats.GetInstance().GetUnlockProgress(caveIndex);

            // Do not spawn a NPC if all NPCs were rescued
            if (unlockProgress >= 3)
                return;

            string npcName = npcUnlocks[caveIndex].unlocks[unlockProgress].npc;

            // Spawn the NPC
            dynamic npcJson = GetNPC(sceneName, npcName);
            LoadNPCData(sceneName, npcJson, false);
        }

        public static void UpdateNPCs()
        {
            SpawnUnlockableNPCs();
        }

        private static void SpawnNPC(string sceneName, string npcName, string texturepath, float textureScale, float x, float y, Option[] options, string[] dialogPages)
        {
            // Create the Entity
            Transform transform = new Transform(new Vector2(x, y), textureScale, 0.0f);
            IEntity entity = new Entity(transform, npcName);
            entity.AddComponent(new NPC(npcName, sceneName));
            entity.AddComponent(new Trigger(TriggerLayerType.FRIEND, new Vector2(-0.4f, -0.4f), 1.4f, 1.8f));
            entity.AddComponent(new Collider(Vector2.Zero, transform.Position, 0.6f, 1f));
            entity.AddComponent(new Interactable(options));
            entity.AddComponent(new Dialog(dialogPages));
            entity.AddComponent(Assets.Asset.GetSprite(texturepath, SpriteLayer.Middleground, true));

            // Spawn it via the manager
            Manager.GetInstance().AddEntityToScene(entity, sceneName);
        }

        private static void LoadNPCData(string sceneName, dynamic npc, bool spawnUnlockableNPC)
        {
            // Spawn all visible npc who are not unlockable (default npcs)
            if (npc.visible == false && spawnUnlockableNPC == false)
                return;

            string npcName = npc.name;
            string texturePath = npc.texturePath;
            float textureScale = npc.textureScale;
            float x = npc.x;
            float y = npc.y;

            // Get the Dialog Array
            JArray dialogArray = npc.dialog;
            string[] dialog = dialogArray.Values<string>().ToArray() !;

            // Get the Option Array
            JArray optionJArray = npc.options;
            Option[] optionArray = new Option[optionJArray.Count];
            int optionIndex = 0;
            foreach (var option in optionJArray)
            {
                string optionType = option.SelectToken("type") !.Value<string>() !;
                string optionString = option.SelectToken("option") !.Value<string>() !;
                string keyString = option.SelectToken("key") !.Value<string>() !;
                Keys key = (Keys)Enum.Parse(typeof(Keys), keyString, true);

                if (optionType == "trade")
                {
                    string neededItem = option.SelectToken("neededItem") !.Value<string>() !;
                    int neededItemAmount = option.SelectToken("neededItemAmount") !.Value<int>() !;
                    string buyItem = option.SelectToken("buyItem") !.Value<string>() !;
                    int buyItemAmount = option.SelectToken("buyItemAmount") !.Value<int>() !;
                    optionArray[optionIndex] = new Option(optionString, "trade", key, neededItem, neededItemAmount, buyItem, buyItemAmount);
                }
                else
                {
                    optionArray[optionIndex] = new Option(optionString, key);
                }

                optionIndex++;
            }

            SpawnNPC(sceneName, npcName, texturePath, textureScale, x, y, optionArray, dialog);
        }
    }
}
