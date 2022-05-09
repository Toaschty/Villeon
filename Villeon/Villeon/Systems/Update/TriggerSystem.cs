﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;

namespace Villeon.Systems
{
    internal class TriggerSystem : System, IUpdateSystem
    {
        public TriggerSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Trigger));
        }

        public void Update(float time)
        {
            List<IEntity> mobEntities = new List<IEntity>();
            List<IEntity> damageEntities = new List<IEntity>();

            for (int i = 0; i < Entities.Count; i++)
            {
                Trigger trigger = Entities[i].GetComponent<Trigger>();

                if (trigger.ToBeRemoved)
                {
                    Manager.GetInstance().RemoveEntity(Entities[i]);
                    i--;
                }
                else
                {
                    Transform transform = Entities[i].GetComponent<Transform>();
                    trigger.Position = transform.Position - trigger.Offset;

                    switch (trigger.Type)
                    {
                        case TriggerType.MOB: mobEntities.Add(Entities[i]); break;
                        case TriggerType.DAMAGE: damageEntities.Add(Entities[i]); break;
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
                        mobEntity.GetComponent<Health>()?.Damage(damageTrigger.Damage);
                        mobEntity.GetComponent<Physics>().Acceleration += damageTrigger.Impulse;
                        damageTrigger.ToBeRemoved = true;
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
