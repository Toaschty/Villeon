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
    public class PlayerDeathSystem : System, IUpdateSystem
    {
        public PlayerDeathSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Player), typeof(Health));
        }

        public void Update(float time)
        {
            foreach (IEntity player in Entities)
            {
                Health health = player.GetComponent<Health>();

                if (health.CurrentHealth <= 0)
                {
                    StateManager.InMenu = true;
                    StateManager.IsPlayerDead = true;
                    health.Heal(0);
                    health.IsInvincible = true;
                }
            }
        }
    }
}
