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
    public class HotbarSystem : System, IUpdateSystem
    {
        public HotbarSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Player), typeof(Health));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                // Get health reference
                Health playerHealth = entity.GetComponent<Health>();

                // Unable to use hotbar items when in menu
                if (StateManager.InMenu)
                    return;

                // Slot 1
                if (KeyHandler.IsPressed(Keys.D1))
                    Hotbar.GetInstance().UseItem(0, playerHealth);

                // Slot 2
                if (KeyHandler.IsPressed(Keys.D2))
                    Hotbar.GetInstance().UseItem(1, playerHealth);

                // Slot 3
                if (KeyHandler.IsPressed(Keys.D3))
                    Hotbar.GetInstance().UseItem(2, playerHealth);

                // Slot 4
                if (KeyHandler.IsPressed(Keys.D4))
                    Hotbar.GetInstance().UseItem(3, playerHealth);
            }
        }
    }
}
