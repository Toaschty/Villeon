using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;

namespace Villeon.Systems.TriggerSystems
{
    public class PortalSystem : TriggerActionSystem, IUpdateSystem
    {
        public PortalSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Trigger), typeof(Player));
            Signature.IncludeAND(typeof(Trigger), typeof(Interactable), typeof(Portal));
        }

        // Add Entity to TriggerLayer & Base
        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);

            Player player = entity.GetComponent<Player>();
            Portal portal = entity.GetComponent<Portal>();
            AddEntityToLayer(player, portal, entity);
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
                    Interactable interactable = collisionPair.Item1.GetComponent<Interactable>();
                    Trigger playerTrigger = collisionPair.Item2.GetComponent<Trigger>();

                    // Check each possible option
                    foreach (Option opt in interactable.Options)
                    {
                        // Skip every options which is not Teleport
                        if (opt.Type != "talk")
                            continue;

                        // Check if corresponding key is pressed
                        if (KeyHandler.IsPressed(opt.Key))
                        {
                            // Teleport the player to another scene!
                            Portal portal = collisionPair.Item1.GetComponent<Portal>();
                            Transform playerTransform = collisionPair.Item2.GetComponent<Transform>();
                            DynamicCollider playerCollider = collisionPair.Item2.GetComponent<DynamicCollider>();
                            playerTransform.Position = portal.PositionToTeleport;
                            playerCollider.LastPosition = portal.PositionToTeleport;
                            SceneLoader.SetActiveScene(portal.SceneToLoad);
                        }
                    }
                }
            }
        }
    }
}
