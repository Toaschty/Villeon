using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.Systems
{
    internal class TriggerSystem : System, IUpdateSystem
    {
        public TriggerSystem(string name)
            : base(name)
        {
            Signature.Include(typeof(Trigger));
        }

        public void Update(float time)
        {
            List<IEntity> mobEntities = new List<IEntity>();
            List<IEntity> damageEntities = new List<IEntity>();
            List<IEntity> portalEntities = new List<IEntity>();

            for (int i = 0; i < Entities.Count; i++)
            {
                Trigger trigger = Entities.ElementAt(i).GetComponent<Trigger>();

                if (trigger.ToBeRemoved)
                {
                    Manager.GetInstance().RemoveEntity(Entities.ElementAt(i));
                    i--;
                }
                else
                {
                    Transform transform = Entities.ElementAt(i).GetComponent<Transform>();
                    trigger.Position = transform.Position - trigger.Offset;

                    switch (trigger.Type)
                    {
                        case TriggerType.MOB: mobEntities.Add(Entities.ElementAt(i)); break;
                        case TriggerType.DAMAGE: damageEntities.Add(Entities.ElementAt(i)); break;
                        case TriggerType.PORTAL: portalEntities.Add(Entities.ElementAt(i)); break;
                    }

                    trigger.Time -= time;
                }
            }

            // Check damage against mob
            foreach (IEntity damageEntity in damageEntities)
            {
                Trigger damageTrigger = damageEntity.GetComponent<Trigger>();

                foreach (IEntity mobEntity in mobEntities)
                {
                    Trigger mobTrigger = mobEntity.GetComponent<Trigger>();
                    if (CollidesAABB(damageTrigger, mobTrigger))
                    {
                        Health health = mobEntity.GetComponent<Health>();
                        health.Damage(damageTrigger.Damage);
                        mobEntity.GetComponent<Physics>().Acceleration += damageTrigger.Impulse;
                        damageTrigger.ToBeRemoved = true;
                    }
                }
            }

            // Check portals against mob
            foreach (IEntity portal in portalEntities)
            {
                Trigger portalTrigger = portal.GetComponent<Trigger>();

                foreach (IEntity mob in mobEntities)
                {
                    if (mob.Name == "Marin")
                    {
                        Trigger playerMobTrigger = mob.GetComponent<Trigger>();
                        if (CollidesAABB(portalTrigger, playerMobTrigger))
                        {
                            if (portalTrigger.SceneName == "DungeonScene")
                            {
                                mob.GetComponent<Transform>().Position = Constants.DUNGEON_SPAWN_POINT;
                                mob.GetComponent<DynamicCollider>().LastPosition = Constants.DUNGEON_SPAWN_POINT;
                            }

                            if (portalTrigger.SceneName == "VillageScene")
                            {
                                mob.GetComponent<Transform>().Position = Constants.VILLAGE_SPAWN_POINT;
                                mob.GetComponent<DynamicCollider>().LastPosition = Constants.VILLAGE_SPAWN_POINT;
                            }

                            SceneLoader.SetActiveScene(portalTrigger.SceneName);
                        }
                    }
                }
            }
        }

        private bool CollidesAABB(Trigger a, Trigger b)
        {
            if (a.Position.X >= (b.Position.X + b.Width) || b.Position.X >= (a.Position.X + a.Width))
                return false;
            if (a.Position.Y >= (b.Position.Y + b.Height) || b.Position.Y >= (a.Position.Y + a.Height))
                return false;
            return true;
        }
    }
}
