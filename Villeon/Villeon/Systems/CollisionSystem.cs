using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Villeon.Components;

namespace Villeon.Systems
{
    public class CollisionSystem : IUpdateSystem
    {
        public CollisionSystem(string name)
        {
            Name = name;
            Signature.Add<Transform>();
            Signature.Add<Collider>();
        }
        public string Name { get; }

        public List<IEntity> Entities { get; private set; } = new();

        public Signature Signature { get; private set; } = new();

        // Collider, Transform, Physics
        public void Update()
        {
            foreach (Entity e in Entities)
            {
                Console.WriteLine(e.Name + " Collision");
                // Do Collision things
            }

            // System braucht ne liste von Entities
            // auf die das System agieren soll
            // Movement System: 
            // Braucht <Transform, Physics>
            // Also brauchts ne liste von Entities, die genau diese Componenten hat.
            // Wie führt das System die Liste?
            

            // Manager.CreateEntity("Player", <Transform, Physics, Collider>);
            // Füge Entity list hinzu
            // Füge Registrierten Systemen diese Hinzu


            // Manager hat
            // Entity list
            // System list with their wanted signatures
            

            // Manager.CreateEntity("Player", SomeSignature);
            // iterate through systems, check signature: true -> Add

            // Manager.CreateSystem()
            // iterate through Entities and snack whats needed


        }
    }
}
