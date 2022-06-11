using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.Systems
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
            foreach (IEntity healthEntity in Entities)
            {
                Health health = healthEntity.GetComponent<Health>();
                Transform transform = healthEntity.GetComponent<Transform>();

                if (health.CurrentHealth <= 0)
                {
                    StateManager.IsPlayerDead = true;
                    // Spawn menu
                    // HOch runter
                    // Continue -> Respawn irgendwo
                    // Quit -> Quit
                    health.Heal(200);
                    healthEntity.GetComponent<Transform>().Position = new Vector2(5f, 5f);
                    healthEntity.GetComponent<DynamicCollider>().LastPosition = new Vector2(5f, 5f);
                    healthEntity.GetComponent<Physics>().Velocity = Vector2.Zero;
                    healthEntity.GetComponent<Physics>().Acceleration = Vector2.Zero;

                    // You died
                    // Continue
                    // Quit

                    StateManager.IsPlayerDead = false;
                }
            }
        }
    }
}
