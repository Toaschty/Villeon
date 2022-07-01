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
    public class GUIInputSystem : System, IUpdateSystem
    {
        private GUIHandler _handler = GUIHandler.GetInstance();
        private Hotbar _hotbar = Hotbar.GetInstance();

        public GUIInputSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(GUIHandler));
        }

        public void Update(float time)
        {
            CheckKeyMenu();

            if (StateManager.InMenu)
            {
                Keys? currentkey = KeyHandler.GetLastPressedKey();
                if (currentkey != null)
                {
                    bool updateMenu = _handler.CurrentMenu!.OnKeyReleased((Keys)currentkey);
                    if (updateMenu)
                    {
                        UnloadMenu(_handler.CurrentMenu);
                        LoadMenu(_handler.CurrentMenu);
                    }
                }
            }
        }

        private void CheckKeyMenu()
        {
            // Death Menu
            if (StateManager.IsPlayerDead && _handler.CurrentMenu != _handler.DeathMenu)
                ChangeMenu(_handler.DeathMenu);

            // Equipment Menu
            if (KeyHandler.IsPressed(Keys.P))
                ChangeMenu(_handler.EquipmentMenu);

            // Inventory Menu
            if (KeyHandler.IsPressed(Keys.Tab) || KeyHandler.IsPressed(Keys.I))
            {
                InventoryMenu.GetInstance().ReloadItemEntities();
                ChangeMenu(_handler.InventoryMenu);
            }

            // Pause Menu
            if (KeyHandler.IsPressed(Keys.Escape))
                ChangeMenu(_handler.PauseMenu);

            // Help Menu
            if (KeyHandler.IsPressed(Keys.H))
                ChangeMenu(_handler.HelpMenu);

            // Lock specific menus in specific scenes
            if (StateManager.InDungeon || StateManager.InTutorial)
                return;

            // Dungeon Menu
            if (KeyHandler.IsPressed(Keys.L))
                ChangeMenu(_handler.DungeonMenu);

            // Map Menu
            if (KeyHandler.IsPressed(Keys.M))
            {
                _handler.MapMenu.MoveViewportToMarker();
                ChangeMenu(_handler.MapMenu);
            }
        }

        private void ChangeMenu(IGUIMenu menu)
        {
            // No menu currently loaded -> Load selected menu
            if (_handler.CurrentMenu == null)
            {
                GUIHandler.GetInstance().CurrentMenu = menu;
                LoadMenu(menu);
                StateManager.InMenu = true;
            }

            // Selected menu already loaded -> Unload menu
            else if (_handler.CurrentMenu == menu)
            {
                UnloadMenu(menu);
                GUIHandler.GetInstance().ClearMenu();
                StateManager.InMenu = false;
            }

            // Other menu currently loaded -> Unload current menu -> Load selected menu
            else
            {
                // don't open other menus while death menu is there or player is dead
                if (_handler.CurrentMenu != _handler.DeathMenu || !StateManager.IsPlayerDead)
                {
                    UnloadMenu(_handler.CurrentMenu);
                    _handler.CurrentMenu = menu;
                    LoadMenu(menu);
                }
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
