using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.ECS;
using Villeon.GUI;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class GUIInputSystem : System, IUpdateSystem
    {
        public GUIInputSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(GUIHandler));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                GUIHandler handler = entity.GetComponent<GUIHandler>();

                CheckKeyMenu(handler);

                if (StateManager.InMenu)
                    handler.CurrentMenu!.OnKeyReleased(KeyHandler.GetLastReleasedKey());
            }
        }

        private void CheckKeyMenu(GUIHandler handler)
        {
            // Dungeon Menu
            if (KeyHandler.WasReleased(Keys.L))
                ChangeMenu(handler, handler.DungeonMenu);

            // Equipment Menu
            if (KeyHandler.WasReleased(Keys.P))
                ChangeMenu(handler, handler.EquipmentMenu);

            // Inventory Menu
            if (KeyHandler.WasReleased(Keys.Tab) || KeyHandler.WasReleased(Keys.I))
                ChangeMenu(handler, handler.InventoryMenu);

            if (KeyHandler.WasReleased(Keys.H))
            {
                InventoryMenu.GetInstance().AddItem(new Item("HealthPotion", Assets.GetSprite("GUI.Potion_Item.png", Render.SpriteLayer.ScreenGuiForeground, false), 12, Item.Type.POTION));
                Console.WriteLine("Spawning Health potion!");
            }
        }

        private void ChangeMenu(GUIHandler handler, IGUIMenu menu)
        {
            // No menu currently loaded -> Load selected menu
            if (handler.CurrentMenu == null)
            {
                handler.CurrentMenu = menu;
                LoadMenu(menu);
                KeyHandler.ClearReleasedKeys();
                StateManager.InMenu = true;
            }

            // Selected menu already loaded -> Unload menu
            else if (handler.CurrentMenu == menu)
            {
                UnloadMenu(menu);
                handler.CurrentMenu = null;
                StateManager.InMenu = false;
            }

            // Other menu currently loaded -> Unload current menu -> Load selected menu
            else
            {
                UnloadMenu(handler.CurrentMenu);
                handler.CurrentMenu = menu;
                LoadMenu(menu);
            }
        }

        // Add entites of menu to current scene
        private void LoadMenu(IGUIMenu menu)
        {
            Manager.GetInstance().AddEntities(menu.GetEntities());
        }

        // Remove entities of menu of current scene
        private void UnloadMenu(IGUIMenu menu)
        {
            Manager.GetInstance().RemoveEntities(menu.GetEntities());
        }
    }
}
