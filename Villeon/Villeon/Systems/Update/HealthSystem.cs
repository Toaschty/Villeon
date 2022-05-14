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
            Signature = Signature.AddToSignature(typeof(Health));
        }

        public void Update(float time)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Health health = Entities[i].GetComponent<Health>();

                if (health.CurrentHealth == 0)
                {
                    // "kill" player
                    if (Entities[i].GetComponent<Player>() != null)
                    {
                        StateManager.IsDead = true;

                        health.Heal(200);
                        Entities[i].GetComponent<Transform>().Position = new Vector2(5f, 5f);
                        Entities[i].GetComponent<Collider>().LastPosition = new Vector2(5f, 5f);
                        Entities[i].GetComponent<Physics>().Velocity = Vector2.Zero;
                        Entities[i].GetComponent<Physics>().Acceleration = Vector2.Zero;

                        StateManager.IsDead = false;
                    }
                    else
                    {
                        Manager.GetInstance().RemoveEntity(Entities[i]);
                        i--;
                    }
                }
            }
        }
    }
}
