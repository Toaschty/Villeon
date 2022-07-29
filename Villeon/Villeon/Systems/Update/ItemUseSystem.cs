using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class ItemUseSystem : System, IUpdateSystem
    {
        public ItemUseSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Player), typeof(Health));
        }

        public void Update(float time)
        {
            foreach (IEntity player in Entities)
            {
                if (KeyHandler.IsPressed(Keys.E) && StateManager.InMenu)
                {
                    Item? selectedItem = InventoryMenu.GetInstance().GetCurrentlySelectedItem();

                    if (selectedItem == null)
                        return;

                    if (selectedItem.ItemType == Item.ITEM_TYPE.POTION)
                    {
                        Health playerHealth = player.GetComponent<Health>();

                        // Check how much current potion heals
                        int health = ItemLoader.GetHealthEffect(selectedItem.Name);
                        if (health > 0)
                            playerHealth.Heal(health);
                        else
                            playerHealth.CurrentHealth = playerHealth.MaxHealth;

                        // Spawn Healed icon!
                        IEntity savingIcon = ParticleBuilder.StationaryParticle(new Vector2(-0.95f, -5f), 1, 0.2f, true, "Animations.Healed.png", Components.SpriteLayer.ScreenGuiOnTopOfForeground);
                        Manager.GetInstance().AddEntity(savingIcon);

                        // Handle inventory & hotbar
                        InventoryMenu.GetInstance().UseItemAtCurrentPosition();
                        Hotbar.GetInstance().UpdateItems();
                    }

                    if (selectedItem.ItemType == Item.ITEM_TYPE.WEAPON)
                    {
                        if (selectedItem.Damage == 0)
                        {
                            // Its a shield!
                            Stats.GetInstance().SetDefenseItem(selectedItem);

                            // Spawn Healed icon!
                            IEntity savingIcon = ParticleBuilder.StationaryParticle(new Vector2(-0.95f, -5f), 1, 0.2f, true, "Animations.Equipped.png", Components.SpriteLayer.ScreenGuiOnTopOfForeground);
                            Manager.GetInstance().AddEntity(savingIcon);

                            EquipmentMenu.GetInstance().AddDefenseWeapon(selectedItem);
                        }

                        if (selectedItem.Defense == 0)
                        {
                            // Spawn Healed icon!
                            IEntity savingIcon = ParticleBuilder.StationaryParticle(new Vector2(-0.95f, -5f), 1, 0.2f, true, "Animations.Equipped.png", Components.SpriteLayer.ScreenGuiOnTopOfForeground);
                            Manager.GetInstance().AddEntity(savingIcon);

                            // Its a weapon!
                            Stats.GetInstance().SetAttackItem(selectedItem);

                            EquipmentMenu.GetInstance().AddAttackWeapon(selectedItem);
                        }
                    }
                }
            }
        }
    }
}
