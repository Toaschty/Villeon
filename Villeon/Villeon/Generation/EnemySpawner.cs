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
        public static void Spawn(string sceneName, string enemyName, Vector2 position)
        {
            // Load the JSON
            JObject enemiesJson = (JObject)JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.Enemies.json")) !;

            // Choose the current scene
            dynamic json = enemiesJson.SelectToken(enemyName) !;

            // Assemble the Enemy //
            float scale = json.scale;
            IEntity enemy = new Entity(new Transform(position, scale, 0f), enemyName);

            // Add Health
            int hp = json.health;
            enemy.AddComponent(new Health(hp));

            // Add Physics
            enemy.AddComponent(new Physics());

            // Add Damage
            int dmg = json.damage;
            enemy.AddComponent(new EnemyAI(dmg));

            float offsetX = json.offsetX;
            float offsetY = json.offsetY;
            Vector2 offset = new Vector2(offsetX, offsetY);

            // Add AI
            string ai = json.ai;
            if (ai.Equals("FlyingAI"))
                enemy.AddComponent(new FlyingAI());
            else if (ai.Equals("JumpingAI"))
                enemy.AddComponent(new JumpingAI());
            else if (ai.Equals("RollingAI"))
                enemy.AddComponent(new RollingAI());

            // Add Experience
            int exp = json.experience;
            enemy.AddComponent(new Exp(exp));

            // Add Physics
            enemy.AddComponent(new Physics());
            enemy.AddComponent(new Effect());

            AddDropInfo(json, enemy);
            AddAnimation(json, enemy);
            AddCollider(json, enemy, position, offset, scale);
            AddTrigger(json, enemy, offset, scale);

            // Spawn the Enemy
            Manager.GetInstance().AddEntityToScene(enemy, sceneName);
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
            JArray animationsArray = json.animations;
            string[] animations = animationsArray.Values<string>().ToArray() !;
            AddSprite(json, entity, animationPath + animations[0]);
            AnimationController animController = new AnimationController();
            foreach (string path in animations)
            {
                animController.AddAnimation(AnimationLoader.CreateAnimationFromFile(animationPath + path, animationSpeed));
            }

            entity.AddComponent(animController);
        }

        private static void AddSprite(dynamic json, IEntity entity, string animationPath)
        {
            Sprite sprite = Asset.GetSpriteSheet(animationPath).GetSprite(0, SpriteLayer.Foreground, true);
            entity.AddComponent(sprite);
        }

        private static void AddCollider(dynamic json, IEntity entity, Vector2 position, Vector2 offset, float scale)
        {
            // Collider
            int colliderWidth = json.colliderWidth / 8f;
            int colliderHeight = json.colliderHeight / 8f;
            entity.AddComponent(new DynamicCollider(offset, position, colliderWidth * scale, colliderHeight * scale));
        }

        private static void AddTrigger(dynamic json, IEntity entity, Vector2 offset, float scale)
        {
            // Trigger
            int triggerWidth = json.triggerWidth / 8f;
            int triggerHeight = json.triggerHeight / 8f;
            entity.AddComponent(new Trigger(TriggerLayerType.ENEMY, offset, triggerWidth * scale, triggerHeight * scale));
        }
    }
}
