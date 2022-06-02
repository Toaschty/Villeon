using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Systems.Update;

namespace Villeon.Systems
{
    public class TriggerActionSystem : System
    {
        private Dictionary<TriggerLayerType, TriggerLayer> _triggerLayers;

        public TriggerActionSystem(string name)
            : base(name)
        {
            _triggerLayers = new Dictionary<TriggerLayerType, TriggerLayer>();
            foreach (Enum value in Enum.GetValues(typeof(TriggerLayerType)))
            {
                _triggerLayers.Add((TriggerLayerType)value, new TriggerLayer());
            }
        }

        public Dictionary<TriggerLayerType, TriggerLayer> TriggerLayers { get { return _triggerLayers; } }

        public void AddEntityToLayer(IComponent receiver, IComponent actor, IEntity entity)
        {
            TriggerLayerType triggerLayerType = entity.GetComponent<Trigger>().TriggerLayers;
            foreach (Enum value in GetSetFlags(triggerLayerType))
            {
                if (receiver is not null)
                {
                    Console.WriteLine("Added player to LadderSystem");
                    _triggerLayers[(TriggerLayerType)value].AddReceiverEntiy(entity);
                }

                if (actor is not null)
                {
                    Console.WriteLine("Added Ladder to LadderSystem");
                    _triggerLayers[(TriggerLayerType)value].AddActingEntiy(entity);
                }
            }
        }

        public override void RemoveEntity(IEntity entity)
        {
            base.RemoveEntity(entity);

            TriggerLayerType triggerLayerType = entity.GetComponent<Trigger>().TriggerLayers;
            foreach (Enum value in GetSetFlags(triggerLayerType))
            {
                _triggerLayers[(TriggerLayerType)value].RemoveEntity(entity);
            }
        }

        public IEnumerable<Enum> GetSetFlags(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
            {
                if (input.HasFlag(value))
                    yield return value;
            }
        }
    }
}
