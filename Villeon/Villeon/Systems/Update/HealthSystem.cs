using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
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
            foreach (IEntity entity in Entities)
            {
                Health health = entity.GetComponent<Health>();
                if (KeyHandler.IsPressed(Keys.L))
                {
                    health.Damage(10);
                    KeyHandler.RemoveKeyHold(Keys.L);
                    Console.WriteLine(health.CurrentHealth);
                }

                if (KeyHandler.IsPressed(Keys.I))
                {
                    health.Heal(10);
                    KeyHandler.RemoveKeyHold(Keys.I);
                    Console.WriteLine(health.CurrentHealth);
                }
            }
        }
    }
}
