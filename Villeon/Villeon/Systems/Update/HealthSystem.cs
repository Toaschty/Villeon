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
    public class HealthSystem : System, IUpdateSystem
    {
        public HealthSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Health));
        }

        public void Update(float time)
        {
            foreach (IEntity healthEntity in Entities)
            {
                Health health = healthEntity.GetComponent<Health>();
                Transform transform = healthEntity.GetComponent<Transform>();

                if (health.CurrentHealth <= 0)
                {
                    if (healthEntity.GetComponent<Player>() is not null)
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
                    else
                    {
                        Manager.GetInstance().RemoveEntity(healthEntity);
                    }
                }
            }

            //for (int i = 0; i < Entities.Count; i++)
            //{
            //    Health health = Entities.ElementAt(i).GetComponent<Health>();
            //    Transform transform = Entities.ElementAt(i).GetComponent<Transform>();

            //    if (health.CurrentHealth == 0)
            //    {
            //        // "kill" player
            //        if (Entities.ElementAt(i).GetComponent<Player>() != null)
            //        {
            //            StateManager.IsPlayerDead = true;

            //            health.Heal(200);
            //            Entities.ElementAt(i).GetComponent<Transform>().Position = new Vector2(5f, 5f);
            //            Entities.ElementAt(i).GetComponent<DynamicCollider>().LastPosition = new Vector2(5f, 5f);
            //            Entities.ElementAt(i).GetComponent<Physics>().Velocity = Vector2.Zero;
            //            Entities.ElementAt(i).GetComponent<Physics>().Acceleration = Vector2.Zero;

            //            StateManager.IsPlayerDead = false;
            //        }
            //        else
            //        {
            //            Manager.GetInstance().RemoveEntity(Entities.ElementAt(i));
            //            i--;
            //        }
            //    }
            //}
        }
    }
}
