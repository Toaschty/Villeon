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
using Zenseless.OpenTK;

namespace Villeon.Generation
{
    public static class NPCLoader
    {
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
                // Get NPC Name, X Position, Y Position
                string npcName = scene[i].name;
                string texturePath = scene[i].texturePath;
                float textureScale = scene[i].textureScale;
                float x = scene[i].x;
                float y = scene[i].y;

                // Get the Dialog Array
                JArray dialogArray = scene[i].dialog;
                string[] dialog = dialogArray.Values<string>().ToArray() !;

                // Get the Option Array
                JArray optionJArray = scene[i].options;
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
                        optionArray[optionIndex] = new Option(optionString, key, neededItem, neededItemAmount, buyItem, buyItemAmount);
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

        private static void SpawnNPC(string sceneName, string npcName, string texturepath, float textureScale, float x, float y, Option[] options, string[] dialogPages)
        {
            // Create the Entity
            Transform transform = new Transform(new Vector2(x, y), textureScale, 0.0f);
            IEntity entity = new Entity(transform, npcName);
            entity.AddComponent(new Trigger(TriggerLayerType.FRIEND, new Vector2(-0.4f, -0.4f), 1.4f, 1.8f));
            entity.AddComponent(new Collider(Vector2.Zero, transform.Position, 0.6f, 1f));
            entity.AddComponent(new Interactable(options));
            entity.AddComponent(new Dialog(dialogPages));
            entity.AddComponent(Assets.Asset.GetSprite(texturepath, SpriteLayer.Middleground, true));

            // Spawn it via the manager
            Manager.GetInstance().AddEntityToScene(entity, sceneName);
        }
    }
}
