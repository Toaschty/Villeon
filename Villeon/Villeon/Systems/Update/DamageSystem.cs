using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;

namespace Villeon.Systems.Update
{
    public class DamageSystem : TriggerActionSystem, IUpdateSystem
    {
        public DamageSystem(string name)
            : base(name)
        {
            Signature.
                IncludeAND(typeof(Trigger), typeof(Health)).
                IncludeAND(typeof(Trigger), typeof(Damage));
        }

        // Add Entity to TriggerLayer & Base
        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            TriggerLayerType triggerLayerType = entity.GetComponent<Trigger>().TriggerLayers;
            Health health = entity.GetComponent<Health>();
            Damage damage = entity.GetComponent<Damage>();
            AddEntityToLayer(health, damage, entity);
        }

        public void Update(float time)
        {
            // Iterate through each Trigger Layer
            foreach (TriggerLayerType layerKey in TriggerLayers.Keys)
            {
                // Continue if there is no collision on this layer
                if (TriggerLayers[layerKey].Collisions is null)
                    continue;

                // Do action when collision happend
                foreach (var collisionPair in TriggerLayers[layerKey].Collisions)
                {
                    Damage damage = collisionPair.Item1.GetComponent<Damage>();
                    Health health = collisionPair.Item2.GetComponent<Health>();
                    health.Damage(damage.Amount);

                    // Remove the DamageEntity
                    RemoveEntity(collisionPair.Item1);
                }
            }
        }
    }
}
