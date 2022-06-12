using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;
using Villeon.Render;
using Villeon.Utils;

namespace Villeon
{
    public class EntitySpawner
    {
        public static void SpawnTrigger(TriggerID id, Vector2 poition, float width, float height)
        {
            IEntity damageEntity = new Entity(new Transform(poition, width, height), "Attack");
            //damageEntity.AddComponent(TriggerBuilder.Build(id));
            Manager.GetInstance().AddEntity(damageEntity);
        }

        public static void SpawnTrigger(TriggerLayerType triggerLayerType, Transform transform)
        {
            IEntity damageEntity = new Entity(transform, "Attack");
            damageEntity.AddComponent(new Trigger(triggerLayerType, new Vector2(2f, 0f), 1f, 1f, 0.1f));
            damageEntity.AddComponent(new Damage(50));
            Manager.GetInstance().AddEntity(damageEntity);
        }

        public void Spawn(Vector2 position)
        {
            IEntity entity = new Entity(new Transform(position, 0.5f, 0f), "Peter");
            entity.AddComponent(new Collider(new Vector2(0f, 0f), position, 2f, 2f));
            entity.AddComponent(new DynamicCollider(entity.GetComponent<Collider>()));
            entity.AddComponent(new Trigger(TriggerLayerType.ENEMY | TriggerLayerType.LADDER, new Vector2(0), 2f, 2f));
            entity.AddComponent(new Health(200));
            entity.AddComponent(new Effect());
            entity.AddComponent(new Physics());
            entity.AddComponent(new EnemyAI());
            entity.AddComponent(new Sprite(SpriteLayer.Foreground, true));

            // Setup player animations
            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.slime_jumping.png", 0.1f));
            entity.AddComponent(animController);
            Manager.GetInstance().AddEntity(entity);
        }
    }
}
