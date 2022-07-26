using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.Systems.TriggerSystems
{
    public class EventSystem : TriggerActionSystem, IUpdateSystem
    {
        public EventSystem(string name)
            : base(name)
        {
            Signature.
                IncludeAND(typeof(Trigger), typeof(Player)).
                IncludeAND(typeof(Trigger), typeof(Event));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);

            Player player = entity.GetComponent<Player>();
            Event interactable = entity.GetComponent<Event>();
            AddEntityToLayer(player, interactable, entity);
        }

        public void Update(float time)
        {
            List<IEntity> entitiesToBeRemoved = new List<IEntity>();

            foreach (TriggerLayerType layerKey in TriggerLayers.Keys)
            {
                // Continue if there is no collision on this layer
                if (TriggerLayers[layerKey].Collisions is null)
                    continue;

                // Do action when collision happend
                foreach (var collisionPair in TriggerLayers[layerKey].Collisions)
                {
                    entitiesToBeRemoved.Add(collisionPair.Item1);
                    Event collisionEvent = collisionPair.Item1.GetComponent<Event>();

                    if (collisionEvent.Name.Equals("BossFall"))
                    {
                        Effect effect = collisionPair.Item2.GetComponent<Effect>();
                        if (effect is not null)
                        {
                            effect.Effects.Add("FokusBoss", 3f);
                            effect.Effects.Add("CameraShake", 3f);
                            effect.Effects.Add("MovemnetDisabled", 3f);
                        }
                    }
                }
            }

            foreach (IEntity entity in entitiesToBeRemoved)
                RemoveEntity(entity);
        }
    }
}
