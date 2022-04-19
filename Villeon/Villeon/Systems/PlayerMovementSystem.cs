using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Villeon.Components;
using OpenTK.Mathematics;

namespace Villeon.Systems
{
    public class PlayerMovementSystem : ISystem
    {
        public PlayerMovementSystem(string name)
        {
            Name = name;
            Signature.Add<Transform>();
            Signature.Add<Physics>();
        }

        public string Name { get; }

        public List<Entity> Entities { get; private set; } = new();

        public Signature Signature { get; private set; } = new();

        public void Update(/* GAMETIME?*/)
        {
            foreach (Entity entity in Entities)
            {
                Console.Write(entity.Name);
                Console.Write(" signature: " + entity.Signature.signature);
                Transform transform = entity.GetComponents<Transform>().First();
                transform.Degrees++;
                transform.Scale++;
                transform.Position += new Vector2(0.1f, 0.1f);
                Console.WriteLine(" degrees: " + transform.Degrees + " scale: " + transform.Scale + " position: " + transform.Position);
            }
        }
    }
}
