using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    internal class VillageEquipSystem : System, IUpdateSystem
    {
        public VillageEquipSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Player));
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
