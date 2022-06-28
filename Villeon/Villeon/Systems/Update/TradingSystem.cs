using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;
using Villeon.Helper;
using Villeon.Systems.TriggerSystems;

namespace Villeon.Systems.Update
{
    public class TradingSystem : TriggerActionSystem, IUpdateSystem
    {
        public TradingSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Trigger), typeof(Player));
            Signature.IncludeAND(typeof(Trigger), typeof(Interactable));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);

            Player player = entity.GetComponent<Player>();
            Interactable interactable = entity.GetComponent<Interactable>();
            AddEntityToLayer(player, interactable, entity);
        }

        public void Update(float time)
        {
            foreach (TriggerLayerType layerKey in TriggerLayers.Keys)
            {
                // Continue if there is no collision on this layer
                if (TriggerLayers[layerKey].Collisions is null || StateManager.InMenu)
                    continue;

                // Do action when collision happend
                foreach (var collisionPair in TriggerLayers[layerKey].Collisions)
                {
                    Interactable interactable = collisionPair.Item1.GetComponent<Interactable>();

                    // Check each possible option
                    foreach (Option opt in interactable.Options)
                    {
                        // Check if corresponding key is pressed
                        if (KeyHandler.IsPressed(opt.Key))
                        {
                            // Check if needed items for trade are in inventory
                            if (InventoryMenu.GetInstance().CheckIfExists(opt.NeededItem, opt.NeededItemAmount))
                            {
                                // Remove needed items for trade out of inventory
                                InventoryMenu.GetInstance().RemoveItems(opt.NeededItem, opt.NeededItemAmount);

                                // Add bought items in inventory
                                InventoryMenu.GetInstance().AddItems(ItemLoader.GetItem(opt.BuyItem), opt.BuyItemAmount);
                            }
                        }
                    }
                }
            }
        }
    }
}
