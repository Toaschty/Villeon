using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;

namespace Villeon.Systems.Update
{
    public class DamageSystem : System, IUpdateSystem
    {
        private TriggerLayer[] _triggerLayers = new TriggerLayer[Enum.GetNames(typeof(TriggerLayerType)).Length];

        public DamageSystem(string name)
            : base(name)
        {
            Signature.
                IncludeAND(typeof(Trigger)).
                ANDEither(typeof(Health), typeof(Damage));

            for (int i = 0; i < _triggerLayers.Count(); i++)
            {
                _triggerLayers[i] = new TriggerLayer();
            }
        }

        // Add Entity to TriggerLayer & Base
        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            TriggerLayerType triggerLayerType = entity.GetComponent<Trigger>().TriggerLayers;

            for (int i = 1; i <= (int)TriggerLayerType.PORTAL; i *= 2)
            {
                if (triggerLayerType.HasFlag((TriggerLayerType)i))
                {
                    if (entity.GetComponent<Health>() is not null)
                        _triggerLayers[i / 2].AddReceiverEntiy(entity);

                    if (entity.GetComponent<Damage>() is not null)
                        _triggerLayers[i / 2].AddActingEntiy(entity);
                }
            }
        }

        // Remove Entity from TriggerLayer & Base
        public override void RemoveEntity(IEntity entity)
        {
            base.RemoveEntity(entity);

            TriggerLayerType triggerLayerType = entity.GetComponent<Trigger>().TriggerLayers;
            for (int i = 1; i <= (int)TriggerLayerType.PORTAL; i *= 2)
            {
                if (triggerLayerType.HasFlag((TriggerLayerType)i))
                {
                    _triggerLayers[i / 2].RemoveEntity(entity);
                }
            }
        }

        public void Update(float time)
        {
            for (int i = 0; i < _triggerLayers.Count(); i++)
            {
                // Go to next Layer if its empty
                if (_triggerLayers[i].Collisions is null)
                    continue;

                foreach (var collisionPair in _triggerLayers[i].Collisions)
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
