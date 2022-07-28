using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation;
using Villeon.GUI;
using Villeon.Helper;

namespace Villeon.Systems.TriggerSystems
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
                    Trigger playerTrigger = collisionPair.Item2.GetComponent<Trigger>();

                    // Check each possible option
                    foreach (Option opt in interactable.Options)
                    {
                        // Check if corresponding key is pressed
                        if (KeyHandler.IsPressed(opt.Key) && opt.Type == "trade")
                        {
                            // Check if needed items for trade are in inventory
                            if (InventoryMenu.GetInstance().CheckIfExists(opt.NeededItem, opt.NeededItemAmount))
                            {
                                // Remove needed items for trade out of inventory
                                InventoryMenu.GetInstance().RemoveItems(opt.NeededItem, opt.NeededItemAmount);

                                // Add bought items in inventory
                                InventoryMenu.GetInstance().AddItems(ItemLoader.GetItem(opt.BuyItem), opt.BuyItemAmount);

                                // Add trade particles
                                List<IEntity> particles = ParticleBuilder.RandomParticles(playerTrigger.Position, new Vector2(0.2f, 0.2f), 2, 0.02f, 0.5f, -0.01f, 0.1f, true, "Sprites.Particles.Sparkles.png", 100, new Vector2(1f, 1.5f), Color4.White);
                                Manager.GetInstance().AddEntities(particles);
                            }
                        }

                        if (KeyHandler.IsPressed(opt.Key) && opt.Type == "level")
                        {
                            // Check if needed items for trade are in inventory
                            if (InventoryMenu.GetInstance().CheckIfExists(opt.NeededItem, opt.NeededItemAmount))
                            {
                                // Remove needed items for trade out of inventory
                                InventoryMenu.GetInstance().RemoveItems(opt.NeededItem, opt.NeededItemAmount);

                                // Upgrade stats
                                switch (opt.UpgradeType)
                                {
                                    case "health": Stats.GetInstance().IncreaseHealth(); break;
                                    case "damage": Stats.GetInstance().IncreaseDamage();  break;
                                    case "defense": Stats.GetInstance().IncreaseDefense(); break;
                                }

                                // Add trade particles
                                List<IEntity> particles = ParticleBuilder.RandomParticles(playerTrigger.Position, new Vector2(0.2f, 0.2f), 2, 0.02f, 0.5f, -0.01f, 0.1f, true, "Sprites.Particles.Sparkles.png", 100, new Vector2(1f, 1.5f), Color4.White);
                                Manager.GetInstance().AddEntities(particles);
                            }
                        }
                    }
                }
            }
        }
    }
}
