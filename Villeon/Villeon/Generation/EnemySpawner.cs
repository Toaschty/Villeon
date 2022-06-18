using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;
using Villeon.Utils;

namespace Villeon.Generation
{
    public static class EnemySpawner
    {
        public static void Spawn(string sceneName, string enemyName, Vector2 position, Vector2 scale)
        {
            // Load the JSON
            JObject enemiesJson = (JObject)JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.Enemies.json"))!;

            // Choose the current scene
            dynamic json = enemiesJson.SelectToken(enemyName) !;

            // Assemble the Enemy //
            IEntity enemy = new Entity(new Transform(position, scale, 0f), enemyName);

            // Add Health
            int hp = json.health;
            enemy.AddComponent(new Health(hp));

            // Add Damage
            int dmg = json.damage;
            enemy.AddComponent(new EnemyAI(dmg));

            AddDropInfo(json, enemy);
            AddAnimation(json, enemy);
            AddCollider(json, enemy, position, scale);
            AddTrigger(json, enemy, scale);

            // Spawn the Enemy
            Manager.GetInstance().AddEntity(enemy);
        }

        private static void AddDropInfo(dynamic json, IEntity entity)
        {
            // Get Drop Info
            int dropCount = json.drops.Count;
            string[] itemNames = new string[dropCount];
            float[] itemDropRates = new float[dropCount];
            int[] minAmounts = new int[dropCount];
            int[] maxAmounts = new int[dropCount];
            for (int i = 0; i < dropCount; i++)
            {
                // Slime drop data
                itemNames[i] = json.drops[i].itemName;
                itemDropRates[i] = json.drops[i].rate;
                minAmounts[i] = json.drops[i].amount.min;
                maxAmounts[i] = json.drops[i].amount.max;
            }

            entity.AddComponent(new DropInfo(dropCount, itemNames, itemDropRates, minAmounts, maxAmounts));
        }

        private static void AddAnimation(dynamic json, IEntity entity)
        {
            // Animation
            string animationPath = json.animationPath;
            float animationSpeed = json.animationSpeed;
            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile(animationPath, animationSpeed));
            entity.AddComponent(animController);
        }

        private static void AddCollider(dynamic json, IEntity entity, Vector2 position, Vector2 scale)
        {
            // Collider
            int colliderWidth = json.colliderWidth / 8f;
            int colliderHeight = json.colliderHeight / 8f;
            entity.AddComponent(new Collider(Vector2.Zero, position, colliderWidth * scale.X, colliderHeight * scale.Y));
        }

        private static void AddTrigger(dynamic json, IEntity entity, Vector2 scale)
        {
            // Trigger
            int triggerWidth = json.triggerWidth / 8f;
            int triggerHeight = json.triggerHeight / 8f;
            entity.AddComponent(new Trigger(TriggerLayerType.ENEMY, triggerWidth * scale.X, triggerHeight * scale.Y));
        }
    }
}
