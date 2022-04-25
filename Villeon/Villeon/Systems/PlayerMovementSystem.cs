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

                if (KeyHandler.IsPressed(Keys.D))
                {
                    physics.Acceleration += new Vector2(Constants.MOVEMENTSPEED, physics.Acceleration.Y);
                }
                if (KeyHandler.IsPressed(Keys.A))
                {
                    physics.Acceleration -= new Vector2(Constants.MOVEMENTSPEED, physics.Acceleration.Y);
                }
                if (KeyHandler.IsPressed(Keys.Space))
                {
                    if (collider.hasCollidedBottom)
                        physics.Velocity = new Vector2(physics.Velocity.X, Constants.JUMPSTRENGTH);
                }
                //Debug Reset Position
                if (KeyHandler.IsPressed(Keys.R))
                {
                    collider.Position = new Vector2(5f, 5f);
                    collider.hasMoved = false;
                    collider.Position = new Vector2(5f, 5f);
                    physics.Velocity = Vector2.Zero;
                }

                if (KeyHandler.IsPressed(Keys.F))
                {
                    physics.Velocity += new Vector2(200f, 0f) * (float)time;
                }

                // Debug Mode
                Constants.DEBUGNEXTFRAME = false;
                if (KeyHandler.IsPressed(Keys.H))
                {
                    Constants.DEBUGPAUSEACTIVE = !Constants.DEBUGPAUSEACTIVE;
                    KeyHandler.RemoveKeyHold(Keys.H);
                }
                if (KeyHandler.IsPressed(Keys.N))
                {
                    Constants.DEBUGNEXTFRAME = true;
                    KeyHandler.RemoveKeyHold(Keys.N);
                }
                if (KeyHandler.IsPressed(Keys.M))
                {
                    Constants.DEBUGNEXTFRAME = true;
                    //KeyHandler.RemoveKeyHold(Keys.N);
                }
                if (KeyHandler.IsPressed(Keys.J))
                {
                    Console.WriteLine("Time: " + Constants.DEBUGTIME + "  Enter new time:");
                    Constants.DEBUGTIME = double.Parse(Console.ReadLine());
                    Console.WriteLine("New time: " + Constants.DEBUGTIME);
                    KeyHandler.RemoveKeyHold(Keys.J);
                }
            }
        }
    }
}
