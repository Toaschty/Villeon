using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Villeon.Components;
using OpenTK.Mathematics;

namespace Villeon.Systems
{
    public class PlayerMovementSystem : IUpdateSystem
    {
        public PlayerMovementSystem(string name)
        {
            Name = name;
            //Signature.Add<Transform>();
            Signature.Add<Collider>();
           // Signature.Add<Physics>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; private set; } = new();

        public Signature Signature { get; private set; } = new();

        public void Update(/* GAMETIME?*/)
        {
            Collider collider;
            foreach (IEntity entity in Entities)
            {
                collider = entity.GetComponent<Collider>();
                collider.Bounds = collider.Bounds.Translated(new Vector2(0.01f, 0.01f));
                Console.WriteLine(collider.Bounds);
            }
        }
    }
}
