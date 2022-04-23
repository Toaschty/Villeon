using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Villeon.Components;
using OpenTK.Mathematics;
using Villeon.Helper;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Villeon.Systems
{
    public class PlayerMovementSystem : IUpdateSystem
    {
        public PlayerMovementSystem(string name)
        {
            Name = name;
            Signature.Add<Physics>();
            Signature.Add<Collider>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; private set; } = new();

        public Signature Signature { get; private set; } = new();

        private float speed = 75.0f;
        private float jumpStrength = 7.5f;

        public void Update(double time)
        {
            Physics physics;
            Collider collider;
            foreach (IEntity entity in Entities)
            {
                physics = entity.GetComponent<Physics>();
                collider = entity.GetComponent<Collider>();

                if (KeyHandler.pressedKeys.Contains(Keys.D))
                {
                    Console.Write("D");
                    physics.Acceleration += new Vector2(speed, physics.Acceleration.Y);
                }
                if (KeyHandler.pressedKeys.Contains(Keys.A))
                {
                    Console.Write("A");
                    physics.Acceleration -= new Vector2(speed, physics.Acceleration.Y);
                }
                if (KeyHandler.pressedKeys.Contains(Keys.Space))
                {
                    Console.Write("Space");
                    if (collider.hasCollidedBottom)
                        physics.Velocity = new Vector2(physics.Velocity.X, jumpStrength);
                }
                //Debug Reset Position
                if (KeyHandler.pressedKeys.Contains(Keys.R))
                {
                    collider.Position = new Vector2(5f, 5f);
                    collider.Position = new Vector2(5f, 5f);
                    physics.Velocity = Vector2.Zero;
                }
            }
        }
    }
}
