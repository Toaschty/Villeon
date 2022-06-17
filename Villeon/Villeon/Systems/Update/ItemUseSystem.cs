using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (KeyHandler.IsPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.U))
                {
                    Item selectedItem = GUIHandler.GetInstance().InventoryMenu.GetCurrentlySelectedItem();
                    if (selectedItem.ItemType == Item.ITEM_TYPE.POTION)
                    {
                        Health playerHealth = player.GetComponent<Health>();
                        playerHealth.Heal(50);
                    }
                }
            }
        }
    }
}
