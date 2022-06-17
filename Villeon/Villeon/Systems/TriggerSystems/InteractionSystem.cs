using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;
using Villeon.Helper;

namespace Villeon.Systems.TriggerSystems
{
    public class InteractionSystem : TriggerActionSystem, IUpdateSystem
    {
        private InteractionPopup? _interactionPopup = null;

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
            if (_interactionPopup is not null)
            {
                _interactionPopup.Delete();
                _interactionPopup = null;
            }

            // Iterate through each Trigger Layer
            foreach (TriggerLayerType layerKey in TriggerLayers.Keys)
            {
                // Continue if there is no collision on this layer
                if (TriggerLayers[layerKey].Collisions is null)
                {
                    continue;
                }

                // Do action when collision happend
                foreach (var collisionPair in TriggerLayers[layerKey].Collisions)
                {
                    // If there is no popup currently -> Spawn it
                    if (_interactionPopup is null)
                    {
                        Transform receiverTransform = collisionPair.Item2.GetComponent<Transform>();
                        Interactable actorInteractable = collisionPair.Item1.GetComponent<Interactable>();

                        // Create Interaction Popup
                        _interactionPopup = new InteractionPopup(receiverTransform.Position, actorInteractable, SpriteLayer.GUIBackground, SpriteLayer.GUIMiddleground);
                        _interactionPopup.Spawn();
                    }

                    // With a popup present -> check all the keys inside the options of the popup
                    if (_interactionPopup is not null)
                    {
                        foreach (Option option in _interactionPopup.Options)
                        {
                            if (KeyHandler.IsPressed(option.Key))
                            {
                                Console.WriteLine("User presed: " + option.Key);
                            }
                        }
                    }
                }
            }
        }
    }
}
