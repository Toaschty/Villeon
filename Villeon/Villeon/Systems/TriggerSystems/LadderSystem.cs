using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;

namespace Villeon.Systems.TriggerSystems
{
    public class LadderSystem : TriggerActionSystem, IUpdateSystem
    {
        public LadderSystem(string name)
            : base(name)
        {
            Signature.
                IncludeAND(typeof(Trigger), typeof(Physics)).
                IncludeAND(typeof(Trigger), typeof(Ladder));
        }

        // Add Entity to TriggerLayer & Base
        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            Physics player = entity.GetComponent<Physics>();
            Ladder ladder = entity.GetComponent<Ladder>();
            AddEntityToLayer(player, ladder, entity);
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
                    // Player is not Ladder
                    // Player gets the option to be able to press up
                    // player goes up
                    if (KeyHandler.IsHeld(Keys.W))
                    {
                        Physics playerPhysics = collisionPair.Item2.GetComponent<Physics>();
                        playerPhysics.Velocity = new Vector2(playerPhysics.Velocity.X, 5f);
                    }
                }
            }
        }
    }
}
