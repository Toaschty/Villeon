﻿using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<IEntity> Entities { get; } = new();

        public Signature Signature { get; } = new();

        private float friction = 7.5f;
        private float gravity = 10.0f;

        public void Update(double time)
        {
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
                if ((collider.hasCollidedBottom && physics.Velocity.Y < 0.0f) ||
                    (collider.hasCollidedTop && physics.Velocity.Y > 0.0f))
                    physics.Velocity = new Vector2(physics.Velocity.X, 0.0f);

                if ((collider.hasCollidedLeft && physics.Velocity.X < 0.0f) ||
                   (collider.hasCollidedRight && physics.Velocity.X > 0.0f))
                    physics.Velocity = new Vector2(0.0f, physics.Velocity.Y);


                // Add Gravity
                physics.Acceleration += new Vector2(0.0f, -gravity);

                // Peak Super downward mega speed speed lets go
                if (physics.Velocity.Y <= 0)
                    physics.Acceleration += new Vector2(0.0f, 2.0f * -gravity);


                // Friction
                physics.Acceleration += new Vector2(-friction * physics.Velocity.X, physics.Acceleration.Y);

                // Physics calculation
                Vector2 oldVelocity = physics.Velocity;
                physics.Velocity += physics.Acceleration * (float)time;
                transform.Position += 0.5f * (oldVelocity + physics.Velocity) * (float)time;
                collider.Position += 0.5f * (oldVelocity + physics.Velocity) * (float)time;


                physics.Acceleration = Vector2.Zero;
            }
        }
    }
}
