using System;
using System.Collections.Generic;
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
            // Hide the InteractionPopup when the player is unable to do much - Dialog, Menu, etc.
            if (!StateManager.IsPlaying)
            {
                if (_interactionPopup is not null)
                    _interactionPopup.Delete();
                return;
            }

            if (StateManager.HasTeleported)
            {
                StateManager.HasTeleported = false;
                if (_interactionPopup is not null)
                {
                    _interactionPopup.DeleteFromScene(StateManager.PreviousSceneName);
                    _interactionPopup = null;
                }

                return;
            }

            // If Not in range delete the popup and disable interactions
            if (_interactionPopup is not null)
            {
                _interactionPopup.Interactable.CanInteract = false;
                _interactionPopup.Delete();
                _interactionPopup = null;
            }

            // Iterate through each Trigger Layer
            foreach (TriggerLayerType layerKey in TriggerLayers.Keys)
            {
                // Continue if there is no collision on this layer
                if (TriggerLayers[layerKey].Collisions is null)
                    continue;

                // Do action when collision happend
                foreach (var collisionPair in TriggerLayers[layerKey].Collisions)
                {
                    // If there is no popup currently -> Spawn it
                    if (_interactionPopup is null)
                    {
                        Transform receiverTransform = collisionPair.Item2.GetComponent<Transform>();
                        Interactable actorInteractable = collisionPair.Item1.GetComponent<Interactable>();

                        // Other Systems with this Entity can now do its Interaction (Dialog, Shop, ..)
                        actorInteractable.CanInteract = true;

                        // Create Interaction Popup
                        _interactionPopup = new InteractionPopup(receiverTransform.Position, actorInteractable, SpriteLayer.GUIBackground, SpriteLayer.GUIMiddleground);
                        _interactionPopup.Spawn();
                    }
                }
            }
        }
    }
}
