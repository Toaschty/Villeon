﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.Helper;

namespace Villeon.Systems
{
    public class PlayerMovementSystem : System, IUpdateSystem
    {
        public PlayerMovementSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Physics));
            Signature = Signature.AddToSignature(typeof(Collider));
            Signature = Signature.AddToSignature(typeof(Player));
        }

        public void Update(float time)
        {
            Physics physics;
            Collider collider;
            Transform transform;
            foreach (IEntity entity in Entities)
            {
                physics = entity.GetComponent<Physics>();
                collider = entity.GetComponent<Collider>();
                transform = entity.GetComponent<Transform>();

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
                    if (collider.HasCollidedBottom)
                        physics.Velocity = new Vector2(physics.Velocity.X, Constants.JUMPSTRENGTH);
                }

                //Debug Reset Position
                if (KeyHandler.IsPressed(Keys.R))
                {
                    transform.Position = new Vector2(5f, 5f);
                    collider.LastPosition = new Vector2(5f, 5f);
                    physics.Velocity = Vector2.Zero;
                }

                if (KeyHandler.IsPressed(Keys.F))
                {
                    physics.Velocity += new Vector2(200f, 0f) * time;
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
                }

                if (KeyHandler.IsPressed(Keys.J))
                {
                    Console.WriteLine("Time: " + Constants.DEBUGTIME + "  Enter new time:");

                    string? newTime = Console.ReadLine();
                    if (newTime == null)
                    {
                        newTime = "0.00833";
                    }

                    Constants.DEBUGTIME = float.Parse(newTime);
                    Console.WriteLine("New time: " + Constants.DEBUGTIME);
                    KeyHandler.RemoveKeyHold(Keys.J);
                }
            }
        }
    }
}
