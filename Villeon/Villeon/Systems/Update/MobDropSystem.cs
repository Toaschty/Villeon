using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Villeon.Utils;

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
            foreach (IEntity enemyEntity in Entities)
            {
                Health health = enemyEntity.GetComponent<Health>();

                // Enemy is dead
                if (health.CurrentHealth <= 0)
                {
                    Random rand = new Random();
                    DropInfo dropInfo = enemyEntity.GetComponent<DropInfo>();
                    for (int i = 0; i < dropInfo.DropCount; i++)
                    {
                        float rate = dropInfo.DropRates[i];
                        int min = dropInfo.MinAmounts[i];
                        int max = dropInfo.MaxAmounts[i];
                        string itemName = dropInfo.ItemNames[i];

                        // Does anything drop? -- 100% = 1, 0% = 0, 50% = 0.5, etc.
                        if (((float)rand.NextDouble()) < rate)
                        {
                            // We dropping bois
                            int amount = rand.Next(min, max);

                            for (int j = 0; j < amount; j++)
                            {
                                Item item = ItemDrops.GetItem(itemName);

                                IEntity drop = CreateDrop(enemyEntity, item);
                                Manager.GetInstance().AddEntity(drop);
                            }
                        }
                    }
                }
            }
        }

        private IEntity CreateDrop(IEntity enemyEntity, Item item)
        {
            // Set the Transform
            Transform transformCopy = new Transform(enemyEntity.GetComponent<Transform>());
            transformCopy.Position += new Vector2(0, 1f);
            transformCopy.Scale = new Vector2(0.25f);

            // Create Entity
            IEntity drop = new Entity(transformCopy, "[Loot][" + item.Name + "]");

            // Add Collider, Trigger, Sprite
            drop.AddComponent(new Collider(new Vector2(0), transformCopy, 1f, 1f));
            drop.AddComponent(new DynamicCollider(drop.GetComponent<Collider>()));
            drop.AddComponent(new Trigger(TriggerLayerType.MOBDROP, 1f, 1f));

            // Add the itemSprite
            drop.AddComponent(item.Sprite);

            // Add fancy physics
            Physics physics = new Physics();
            Random random = new Random();
            physics.Velocity = new Vector2(random.Next(-20, 20), 5);
            drop.AddComponent(physics);

            // Add the Drop with the item
            drop.AddComponent(new Drop(item));
            return drop;
        }
    }
}
