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
            Signature.Include(typeof(Health));
        }

        public void Update(float time)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Health health = Entities.ElementAt(i).GetComponent<Health>();
                Transform transform = Entities.ElementAt(i).GetComponent<Transform>();

                if (health.CurrentHealth == 0)
                {
                    // "kill" player
                    if (Entities.ElementAt(i).GetComponent<Player>() != null)
                    {
                        StateManager.IsDead = true;

                        health.Heal(200);
                        Entities.ElementAt(i).GetComponent<Transform>().Position = new Vector2(5f, 5f);
                        Entities.ElementAt(i).GetComponent<DynamicCollider>().LastPosition = new Vector2(5f, 5f);
                        Entities.ElementAt(i).GetComponent<Physics>().Velocity = Vector2.Zero;
                        Entities.ElementAt(i).GetComponent<Physics>().Acceleration = Vector2.Zero;

                        StateManager.IsDead = false;
                    }
                    else
                    {
                        Manager.GetInstance().RemoveEntity(Entities.ElementAt(i));
                        i--;
                    }
                }
            }
        }
    }
}
