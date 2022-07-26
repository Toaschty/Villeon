using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.Systems.Update
{
    public class PlayerDungeonMovementSystem : System, IUpdateSystem
    {
        public PlayerDungeonMovementSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Physics), typeof(DynamicCollider), typeof(Player), typeof(Effect));
        }

        public void Update(float time)
        {
            // Cant move when In Menu or Dialog
            if (StateManager.InMenu || StateManager.InDialog)
                return;

            Effect effect;
            Physics physics;
            Collider collider;
            DynamicCollider dynamicCollider;
            Transform transform;
            Player playerComponent;
            foreach (IEntity player in Entities)
            {
                physics = player.GetComponent<Physics>();
                collider = player.GetComponent<Collider>();
                dynamicCollider = player.GetComponent<DynamicCollider>();
                transform = player.GetComponent<Transform>();
                playerComponent = player.GetComponent<Player>();
                effect = player.GetComponent<Effect>();

                // Player is not moving
                playerComponent.MovingLeft = false;
                playerComponent.MovingRight = false;
                playerComponent.IsWalking = false;

                // Check if player is grounded
                StateManager.IsGrounded = dynamicCollider.HasCollidedBottom;

                // Player is Walking Right
                if (KeyHandler.IsHeld(Keys.D))
                {
                    playerComponent.IsWalking = true;
                    playerComponent.MovingRight = true;
                    playerComponent.MovingLeft = false;
                    playerComponent.WasLookingRight = true;
                    playerComponent.WasLookingLeft = false;
                    physics.Acceleration += new Vector2(Constants.MOVEMENTSPEED, physics.Acceleration.Y);
                }

                // Player is Walking Left
                if (KeyHandler.IsHeld(Keys.A))
                {
                    playerComponent.IsWalking = true;
                    playerComponent.MovingLeft = true;
                    playerComponent.MovingRight = false;
                    playerComponent.WasLookingRight = false;
                    playerComponent.WasLookingLeft = true;
                    physics.Acceleration -= new Vector2(Constants.MOVEMENTSPEED, physics.Acceleration.Y);
                }

                if (!effect.Effects.ContainsKey("DashCooldown"))
                {
                    if (KeyHandler.IsPressed(Keys.LeftShift))
                    {
                        effect.Effects.Add("DashCooldown", 1f);
                        int direction = playerComponent.WasLookingLeft ? -1 : 1;
                        physics.Velocity = new Vector2(Constants.DASH_POWER * direction, physics.Velocity.Y);

                        // Spawn some dash particles in opposite direction
                        List<IEntity> dashParticle = ParticleBuilder.RandomParticles(transform.Position, new Vector2(0.5f, 1.5f), 1f, 0.5f, new Vector2(direction * 5f, 0f), -0.01f, 0.2f, true, "Sprites.Particles.Stripe.png", 25, new Vector2(2f, 3f), Color4.White);
                        Manager.GetInstance().AddEntities(dashParticle);
                    }
                }

                // Jump
                if (KeyHandler.IsHeld(Keys.Space))
                {
                    if (StateManager.IsGrounded)
                    {
                        StateManager.IsGrounded = false;
                        physics.Velocity = new Vector2(physics.Velocity.X, Constants.JUMPSTRENGTH);

                        // Spawn some jump particles
                        List<IEntity> jumpParticles = ParticleBuilder.RandomParticles(transform.Position, new Vector2(0.5f, 0f), 1f, 0.5f, 1f, -0.01f, 0, true, "Sprites.Particles.Dust.png", 25, new Vector2(3, 0), Color4.White);
                        Manager.GetInstance().AddEntities(jumpParticles);
                    }
                }

                //Debug Reset Position
                if (KeyHandler.IsPressed(Keys.R))
                {
                    transform.Position = new Vector2(5f, 80f);
                    dynamicCollider.LastPosition = new Vector2(5f, 80f);
                    physics.Velocity = Vector2.Zero;
                }

                Console.WriteLine("Player Dungeon Position" + transform.Position);
            }
        }
    }
}
