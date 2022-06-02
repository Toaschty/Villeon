using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;

namespace Villeon.Systems.Update
{
    public class PortalSystem : TriggerActionSystem, IUpdateSystem
    {
        public PortalSystem(string name)
            : base(name)
        {
            Signature.
                IncludeAND(typeof(Trigger), typeof(Player)).
                IncludeAND(typeof(Trigger), typeof(Portal));
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
