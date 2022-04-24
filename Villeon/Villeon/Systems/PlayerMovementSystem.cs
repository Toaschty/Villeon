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
            Signature.Add<Player>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; private set; } = new();

        public Signature Signature { get; private set; } = new();

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
                    physics.Acceleration += new Vector2(Constants.MOVEMENTSPEED, physics.Acceleration.Y);
                }
                if (KeyHandler.pressedKeys.Contains(Keys.A))
                {
                    physics.Acceleration -= new Vector2(Constants.MOVEMENTSPEED, physics.Acceleration.Y);
                }
                if (KeyHandler.pressedKeys.Contains(Keys.Space))
                {
                    if (collider.hasCollidedBottom)
                        physics.Velocity = new Vector2(physics.Velocity.X, Constants.JUMPSTRENGTH);
                }
                //Debug Reset Position
                if (KeyHandler.pressedKeys.Contains(Keys.R))
                {
                    collider.Position = new Vector2(5f, 5f);
                    collider.Position = new Vector2(5f, 5f);
                    physics.Velocity = Vector2.Zero;
                }

                if (KeyHandler.pressedKeys.Contains(Keys.F))
                {
                    collider.SetPosition(collider.Position + new Vector2(1f, 0f));
                }
            }
        }
    }
}
