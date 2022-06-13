﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.Systems
{
    public class PlayerMovementSystem : System, IUpdateSystem
    {
        public PlayerMovementSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Physics), typeof(Collider), typeof(Player));
        }

        public void Update(float time)
        {
            Physics physics;
            Collider collider;
            DynamicCollider dynamicCollider;
            Transform transform;
            foreach (IEntity player in Entities)
            {
                physics = player.GetComponent<Physics>();
                collider = player.GetComponent<Collider>();
                dynamicCollider = player.GetComponent<DynamicCollider>();
                transform = player.GetComponent<Transform>();

                // Check if player is grounded
                StateManager.IsGrounded = collider.HasCollidedBottom;

                if (KeyHandler.IsHeld(Keys.D))
                {
                    physics.Acceleration += new Vector2(Constants.MOVEMENTSPEED, physics.Acceleration.Y);
                }

                if (KeyHandler.IsHeld(Keys.A))
                {
                    physics.Acceleration -= new Vector2(Constants.MOVEMENTSPEED, physics.Acceleration.Y);
                }

                if (KeyHandler.IsHeld(Keys.Space))
                {
                    if (StateManager.IsGrounded)
                    {
                        StateManager.IsGrounded = false;
                        physics.Velocity = new Vector2(physics.Velocity.X, Constants.JUMPSTRENGTH);
                    }
                }

                if (KeyHandler.IsPressed(Keys.E))
                {
                    Effect effect = player.GetComponent<Effect>() !;
                    if (!effect.Effects.ContainsKey("AttackCooldown"))
                    {
                        IEntity attackEntity;
                        attackEntity = new Entity(transform, "AttackRight");
                        attackEntity.AddComponent(new Trigger(TriggerLayerType.ENEMY, new Vector2(2f, 0f), 2f, 2f, 0.2f));
                        Sprite sprite = Assets.GetSprite("Sprites.Empty.png", Render.SpriteLayer.GUIForeground, true);
                        sprite.Offset = new Vector2(1, 0);

                        // Setup player animations
                        AnimationController animController = new AnimationController();
                        animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.slash_attack_right.png", 0.05f));
                        attackEntity.AddComponent(animController);
                        attackEntity.AddComponent(sprite);
                        attackEntity.AddComponent(new Damage(50));
                        Manager.GetInstance().AddEntity(attackEntity);
                        effect.Effects.Add("AttackCooldown", 0.1f);
                    }
                }

                if (KeyHandler.IsPressed(Keys.Q))
                {
                    Effect effect = player.GetComponent<Effect>() !;
                    if (!effect.Effects.ContainsKey("AttackCooldown"))
                    {
                        IEntity attackEntity;
                        attackEntity = new Entity(transform, "AttackLeft");
                        attackEntity.AddComponent(new Trigger(TriggerLayerType.ENEMY, new Vector2(-3f, 0f), 2f, 2f, 0.2f));
                        Sprite sprite = Assets.GetSprite("Sprites.Empty.png", Render.SpriteLayer.GUIForeground, true);
                        sprite.Offset = new Vector2(-2, 0);

                        // Setup player animations
                        AnimationController animController = new AnimationController();
                        animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.slash_attack_left.png", 0.05f));
                        attackEntity.AddComponent(animController);
                        attackEntity.AddComponent(sprite);
                        attackEntity.AddComponent(new Damage(50));
                        Manager.GetInstance().AddEntity(attackEntity);
                        effect.Effects.Add("AttackCooldown", 0.1f);
                    }
                }

                //Debug Reset Position
                if (KeyHandler.IsPressed(Keys.R))
                {
                    transform.Position = new Vector2(5f, 5f);
                    dynamicCollider.LastPosition = new Vector2(5f, 5f);
                    physics.Velocity = Vector2.Zero;
                }

                if (KeyHandler.IsPressed(Keys.F))
                {
                    physics.Velocity += new Vector2(200f, 0f) * time;
                }

                // Debug Mode
                StateManager.DEBUGNEXTFRAME = false;
                if (KeyHandler.IsPressed(Keys.H))
                {
                    StateManager.DEBUGPAUSEACTIVE = !StateManager.DEBUGPAUSEACTIVE;
                }

                if (KeyHandler.IsPressed(Keys.N))
                {
                    StateManager.DEBUGNEXTFRAME = true;
                }

                if (KeyHandler.IsPressed(Keys.M))
                {
                    StateManager.DEBUGNEXTFRAME = true;
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
                }
            }
        }
    }
}