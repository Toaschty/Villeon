using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class PhysicsSystem : System, IUpdateSystem
    {
        private IEntity? _player = null;

        public PhysicsSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Physics)).
                IncludeAND(typeof(Physics), typeof(Player));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            if (entity.HasComponent<Player>())
            {
                _player = entity;
            }
        }

        public void Update(float time)
        {
            Physics physics;
            Transform transform;
            DynamicCollider collider;

            foreach (IEntity entity in Entities)
            {
                // Get Components
                physics = entity.GetComponent<Physics>();
                transform = entity.GetComponent<Transform>();
                collider = entity.GetComponent<DynamicCollider>();

                // Skip this entity if player isn't in range
                if (!IsInRangeOfPlayer(transform))
                    continue;

                if (collider != null)
                {
                    // If Collided, stop player in that axis
                    if ((collider.HasCollidedBottom && physics.Velocity.Y < 0.0f) ||
                        (collider.HasCollidedTop && physics.Velocity.Y > 0.0f))
                        physics.Velocity = new Vector2(physics.Velocity.X, 0.0f);

                    if ((collider.HasCollidedLeft && physics.Velocity.X < 0.0f) ||
                       (collider.HasCollidedRight && physics.Velocity.X > 0.0f))
                        physics.Velocity = new Vector2(0.0f, physics.Velocity.Y);
                }

                // Add Gravity to acceleration
                physics.Acceleration += new Vector2(0.0f, -Constants.GRAVITY * physics.Weight);

                // Peak downward acceleration
                if (physics.Velocity.Y <= 0)
                    physics.Acceleration += new Vector2(0.0f, 2.0f * -Constants.GRAVITY * physics.Weight);

                // Friction
                physics.Acceleration += new Vector2(-Constants.FRICTION * physics.Velocity.X * physics.Friction, 0);

                // Physics calculation
                Vector2 oldVelocity = physics.Velocity;
                physics.Velocity += physics.Acceleration * time;
                transform.Position += 0.5f * (oldVelocity + physics.Velocity) * time;

                physics.Acceleration = Vector2.Zero;
            }
        }

        private bool IsInRangeOfPlayer(Transform physicsTransform)
        {
            if (_player is null)
                return true;

            Transform playerTransform = _player.GetComponent<Transform>();
            Vector2 distance = playerTransform.Position - physicsTransform.Position;

            // Length between player and enemy
            float length = distance.LengthFast;
            if (length < 15)
                return true;

            return false;
        }
    }
}
