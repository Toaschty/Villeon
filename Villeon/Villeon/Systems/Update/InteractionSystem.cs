
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class InteractionSystem : TriggerActionSystem, IUpdateSystem
    {
        public InteractionSystem(string name)
            : base(name)
        {
            Signature.
                IncludeAND(typeof(Trigger), typeof(Player)).
                IncludeAND(typeof(Trigger), typeof(Interactable));
        }

        // Add Entity to TriggerLayer & Base
        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);

            Player player = entity.GetComponent<Player>();
            Interactable interactable = entity.GetComponent<Interactable>();
            AddEntityToLayer(player, interactable, entity);
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
                    if (KeyHandler.IsHeld(Keys.E))
                        Console.WriteLine("Collision: " + collisionPair.Item1.Name + " and " + collisionPair.Item2.Name);
                }
            }
        }
    }
}
