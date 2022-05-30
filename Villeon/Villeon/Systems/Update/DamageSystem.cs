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
                Include(typeof(Trigger)).
                AndEither(typeof(Health), typeof(Damage));

            for (int i = 0; i < _triggerLayers.Count(); i++)
            {
                _triggerLayers[i] = new TriggerLayer();
            }
        }

        // Add Entity to TriggerLayer & Base
        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            int layerIndex = (int)entity.GetComponent<Trigger>().LayerType;

            if (entity.GetComponent<Health>() is not null)
                _triggerLayers[layerIndex].AddReceiverEntiy(entity);

            if (entity.GetComponent<Damage>() is not null)
                _triggerLayers[layerIndex].AddActingEntiy(entity);

            Console.WriteLine("Added: " + entity.Name);
        }

        // Remove Entity from TriggerLayer & Base
        public override void RemoveEntity(IEntity entity)
        {
            base.RemoveEntity(entity);

            int layerIndex = (int)entity.GetComponent<Trigger>().LayerType;
            _triggerLayers[layerIndex].RemoveEntity(entity);
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
