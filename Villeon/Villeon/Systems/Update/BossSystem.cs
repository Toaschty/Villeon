using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.Systems.Update
{
    public class BossSystem : System, IUpdateSystem
    {
        public BossSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Boss), typeof(Health));
        }

        public void Update(float time)
        {
            foreach (IEntity boss in Entities)
            {
                // Is Boss dead?
                if (boss.GetComponent<Health>().CurrentHealth <= 0)
                {
                    // Spawn NPC
                    Console.WriteLine("Spawning NPC!");

                    // Spawn Portal
                    Console.WriteLine("Spawning Portal!");
                }
            }
        }
    }
}
