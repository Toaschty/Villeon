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
            Signature.IncludeAND(typeof(GUIHandler));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                GUIHandler handler = GUIHandler.GetInstance();

                CheckKeyMenu(handler);

                if (StateManager.InMenu)
                {
                    Keys? currentkey = KeyHandler.GetLastPressedKey();
                    if (currentkey != null)
                    {
                        bool updateMenu = handler.CurrentMenu!.OnKeyReleased((Keys)currentkey);
                        if (updateMenu)
                        {
                            UnloadMenu(handler.CurrentMenu);
                            LoadMenu(handler.CurrentMenu);
                        }
                    }
                }
            }
        }

        private void CheckKeyMenu(GUIHandler handler)
        {
            // Dungeon Menu
            if (KeyHandler.IsPressed(Keys.L))
                ChangeMenu(handler, handler.DungeonMenu);

            // Equipment Menu
            if (KeyHandler.IsPressed(Keys.P))
                ChangeMenu(handler, handler.EquipmentMenu);

            // Inventory Menu
            if (KeyHandler.IsPressed(Keys.Tab) || KeyHandler.IsPressed(Keys.I))
                ChangeMenu(handler, handler.InventoryMenu);

            // Map Menu
            if (KeyHandler.IsPressed(Keys.M))
            {
                handler.MapMenu.MoveViewportToMarker();
                ChangeMenu(handler, handler.MapMenu);
            }

            // Pause Menu
            if (KeyHandler.IsPressed(Keys.Escape))
                ChangeMenu(handler, handler.PauseMenu);
        }

        private void ChangeMenu(GUIHandler handler, IGUIMenu menu)
        {
            // No menu currently loaded -> Load selected menu
            if (handler.CurrentMenu == null)
            {
                GUIHandler.GetInstance().CurrentMenu = menu;
                LoadMenu(menu);
                StateManager.InMenu = true;
            }

            // Selected menu already loaded -> Unload menu
            else if (handler.CurrentMenu == menu)
            {
                UnloadMenu(menu);
                GUIHandler.GetInstance().ClearMenu();
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
