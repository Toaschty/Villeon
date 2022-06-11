using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class MobDropSystem : System, IUpdateSystem
    {
        public MobDropSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Health), typeof(EnemyAI), typeof(Trigger));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                Health health = entity.GetComponent<Health>();

                // Enemy is dead
                if (health.CurrentHealth <= 0)
                {
                    Transform transformCopy = new Transform(entity.GetComponent<Transform>());
                    transformCopy.Position += new Vector2(0, 1f);
                    transformCopy.Scale = new Vector2(0.25f);
                    IEntity mobDrop = new Entity(transformCopy, "[SlimeLoot][" + entity.Name + "]");
                    mobDrop.AddComponent(new Collider(new Vector2(0), transformCopy, 1f, 1f));
                    mobDrop.AddComponent(new DynamicCollider(mobDrop.GetComponent<Collider>()));
                    mobDrop.AddComponent(new Trigger(TriggerLayerType.MOBDROP, 1f, 1f));
                    mobDrop.AddComponent(Assets.GetSprite("GUI.Potion_Item.png", Render.SpriteLayer.GUIBackground, true));
                    Physics physics = new Physics();
                    Random random = new Random();
                    physics.Velocity = new Vector2(random.Next(-20, 20), 5);
                    mobDrop.AddComponent(physics);
                    mobDrop.AddComponent(new Drops());
                    Manager.GetInstance().AddEntity(mobDrop);

                    // Remove the dead entity
                    Manager.GetInstance().RemoveEntity(entity);
                }
            }
        }
    }
}
