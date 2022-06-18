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

namespace Villeon.Generation
{
    public class NPCLoader
    {
        // Take the npc json
        // place all the npcs
        // shove dialog into em
        public NPCLoader()
        {

        }

        public static void LoadNpcs(string sceneName)
        {
            JObject npcs = (JObject)JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.NPCs.json")) !;
            dynamic scene = npcs.SelectToken(sceneName) !;
            int npcCount = scene.Count;

            // Add the NPCS
            for (int i = 0; i < npcCount; i++)
            {
                // Get NPC Name, X Position, Y Position
                string npcName = scene[i].name;
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
                    string optionString = option.SelectToken("option") !.Value<string>() !;
                    string keyString = option.SelectToken("key") !.Value<string>() !;
                    Keys key = (Keys)Enum.Parse(typeof(Keys), keyString, true);
                    optionArray[optionIndex] = new Option(optionString, key);
                    optionIndex++;
                }

                SpawnNPC(sceneName, npcName, x, y, optionArray, dialog);
            }
        }

        private static void SpawnNPC(string sceneName, string npcName, float x, float y, Option[] options, string[] dialogPages)
        {
            IEntity entity = new Entity(new Transform(new Vector2(x, y), 1.0f, 0.0f), npcName);
            entity.AddComponent(new Trigger(TriggerLayerType.FRIEND, new Vector2(-1, -1), 2f, 2f));
            entity.AddComponent(new Interactable(options));
            entity.AddComponent(new Dialog(dialogPages));
            entity.AddComponent(Assets.Asset.GetSprite("Sprites.Player.png", SpriteLayer.Foreground, true));

            Manager.GetInstance().AddEntityToScene(entity, sceneName);
        }
    }
}
