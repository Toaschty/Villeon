using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
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
                if (KeyHandler.IsPressed(Keys.U))
                {
                    Item? selectedItem = InventoryMenu.GetInstance().GetCurrentlySelectedItem();

                    if (selectedItem == null)
                        return;

                    if (selectedItem.ItemType == Item.ITEM_TYPE.POTION)
                    {
                        Console.WriteLine("Using Potion!");
                        Health playerHealth = player.GetComponent<Health>();
                        playerHealth.Heal(50);
                        InventoryMenu.GetInstance().RemoveItemAtCurrentPosition();
                    }
                }
            }
        }
    }
}
