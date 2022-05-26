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

                // Dungeon Menu
                if (KeyHandler.IsPressed(Keys.L))
                {
                    ChangeMenu(handler, handler.DungeonMenu);

                    KeyHandler.RemoveKeyHold(Keys.L);
                }

                // Equipment Menu
                if (KeyHandler.IsPressed(Keys.P))
                {
                    ChangeMenu(handler, handler.EquipmentMenu);

                    KeyHandler.RemoveKeyHold(Keys.P);
                }
            }
        }

        private void ChangeMenu(GUIHandler handler, IGUIMenu menu)
        {
            // No menu currently loaded -> Load selected menu
            if (handler.CurrentMenu == null)
            {
                handler.CurrentMenu = menu;
                LoadMenu(menu);
            }

            // Selected menu already loaded -> Unload menu
            else if (handler.CurrentMenu == menu)
            {
                UnloadMenu(menu);
                handler.CurrentMenu = null;
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
