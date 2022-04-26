﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;

namespace Villeon.Systems
{
    public class PhysicsSystem : IUpdateSystem
    {
        public PhysicsSystem(string name)
        {
            Name = name;
            Signature.Add<Physics>();
            Signature.Add<Transform>();
            Signature.Add<Collider>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; } = new ();

        public Signature Signature { get; } = new ();

        public void Update(float time)
        {
            if (Constants.DEBUGPAUSEACTIVE)
            {
                if (Constants.DEBUGNEXTFRAME && Constants.DEBUGTHISFRAMEPHYSICS)
                {
                    time = Constants.DEBUGTIME;
                }
                else
                {
                    return;
                }
            }

            Physics physics;
            Transform transform;
            Collider collider;
            foreach (IEntity entity in Entities)
            {
                // Get Components
                physics = entity.GetComponent<Physics>();
                transform = entity.GetComponent<Transform>();
                collider = entity.GetComponent<Collider>();

                // If Collided, stop player in that axis
                if ((collider.HasCollidedBottom && physics.Velocity.Y < 0.0f) ||
                    (collider.HasCollidedTop && physics.Velocity.Y > 0.0f))
                    physics.Velocity = new Vector2(physics.Velocity.X, 0.0f);

                if ((collider.HasCollidedLeft && physics.Velocity.X < 0.0f) ||
                   (collider.HasCollidedRight && physics.Velocity.X > 0.0f))
                    physics.Velocity = new Vector2(0.0f, physics.Velocity.Y);

                // Add Gravity
                physics.Acceleration += new Vector2(0.0f, -Constants.GRAVITY);

                // Peak Super downward mega speed speed lets go
                if (physics.Velocity.Y <= 0)
                    physics.Acceleration += new Vector2(0.0f, 2.0f * -Constants.GRAVITY);

                // Friction
                physics.Acceleration += new Vector2(-Constants.FRICTION * physics.Velocity.X, physics.Acceleration.Y);

                // Physics calculation
                Vector2 oldVelocity = physics.Velocity;
                physics.Velocity += physics.Acceleration * time;
                transform.Position += 0.5f * (oldVelocity + physics.Velocity) * time;
                collider.Position += 0.5f * (oldVelocity + physics.Velocity) * time;

                physics.Acceleration = Vector2.Zero;
            }
        }
    }
}
