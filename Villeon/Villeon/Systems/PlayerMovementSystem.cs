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
            Signature.Add<Transform>();
            Signature.Add<Physics>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; private set; } = new();

        public Signature Signature { get; private set; } = new();

        public void Update(/* GAMETIME?*/)
        {
            foreach (IEntity entity in Entities)
            {
                Console.Write(entity.Name);
                Console.Write(" signature: " + entity.Signature.signature);
                Transform transform = entity.GetComponents<Transform>().First();
                transform.Degrees++;
                transform.Scale++;
                transform.Position += new Vector2(0.01f, 0.01f);
                Console.WriteLine(" degrees: " + transform.Degrees + " scale: " + transform.Scale + " position: " + transform.Position);
            }
        }
    }
}
