using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class JumpingAISystem : System, IUpdateSystem
    {
        private IEntity? _playerEntity;

        public JumpingAISystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Physics), typeof(DynamicCollider), typeof(EnemyAI), typeof(Effect))
                .IncludeOR(typeof(Player));
        }

        public override void AddEntity(IEntity entity)
        {
            if (entity.GetComponent<Player>() is not null)
            {
                _playerEntity = entity;
                return;
            }

            base.AddEntity(entity);
        }

        public override void RemoveEntity(IEntity entity)
        {
            if (entity.GetComponent<Player>() is not null)
            {
                _playerEntity = null;
                return;
            }

            base.RemoveEntity(entity);
        }

        public void Update(float time)
        {
            if (_playerEntity is null)
                return;

            Transform playerTransform = _playerEntity.GetComponent<Transform>();
            DynamicCollider playerDynamicCollider = _playerEntity.GetComponent<DynamicCollider>();

            foreach (IEntity enemyEntity in Entities)
            {
                Effect effect = enemyEntity.GetComponent<Effect>();
                DynamicCollider enemyCollider = enemyEntity.GetComponent<DynamicCollider>();
                Vector2 direction = GetDirectionTowardsPlayer(enemyEntity);

                if (IsInAttackRange(direction) && enemyCollider.HasCollidedBottom)
                {
                    // Is attack on cooldown
                    if (!effect.Effects.ContainsKey("AttackCooldown"))
                    {
                        Attack(enemyEntity, direction);
                        effect.Effects.Add("AttackCooldown", 1);
                    }
                }

                if (IsPlayerInRange(direction))
                {
                    Move(enemyEntity, direction);
                }
            }
        }

        private void Move(IEntity enemyEntity, Vector2 direction)
        {
            // Enemy movement
            Physics physics = enemyEntity.GetComponent<Physics>();
            DynamicCollider collider = enemyEntity.GetComponent<DynamicCollider>();

            // Move Left
            if (IsPlayerOnLeft(direction))
            {
                Jump(enemyEntity, direction);
                physics.Acceleration += new Vector2(Constants.MOVEMENTSPEED * -1 * 0.01f, physics.Acceleration.Y);

                if (collider.HasCollidedLeft && collider.HasCollidedBottom)
                    physics.Velocity = new Vector2(physics.Velocity.X, Constants.JUMPSTRENGTH);
            }

            // Move Right
            if (IsPlayerOnRight(direction))
            {
                Jump(enemyEntity, direction);
                physics.Acceleration += new Vector2(Constants.MOVEMENTSPEED * 1 * 0.01f, physics.Acceleration.Y);

                if (collider.HasCollidedRight && collider.HasCollidedBottom)
                    physics.Velocity = new Vector2(physics.Velocity.X, Constants.JUMPSTRENGTH);
            }
        }

        private Vector2 GetDirectionTowardsPlayer(IEntity enemyEntity)
        {
            Transform playerTransform = _playerEntity!.GetComponent<Transform>();
            Transform enemyTransform = enemyEntity.GetComponent<Transform>();

            Vector2 direction = enemyTransform.Position - playerTransform.Position;
            return direction;
        }

        private bool IsPlayerOnLeft(Vector2 direction)
        {
            if (direction.X >= 0)
                return true;

            return false;
        }

        private bool IsPlayerOnRight(Vector2 direction)
        {
            if (direction.X < 0)
                return true;

            return false;
        }

        private bool IsPlayerInRange(Vector2 direction)
        {
            if (direction.Length < 10 && direction.Length > 2.2f)
                return true;

            return false;
        }

        private bool IsInAttackRange(Vector2 direction)
        {
            if (direction.Length <= 3)
                return true;

            return false;
        }

        private void Attack(IEntity enemyEntity, Vector2 direction)
        {
            Transform enemyTransform = enemyEntity.GetComponent<Transform>();

            if (IsPlayerOnLeft(direction))
            {
                // Spawn Attack Trigger
                IEntity attackEntity;
                attackEntity = new Entity(new Transform(enemyTransform.Position, 1f, 0), "AttackLeft");
                attackEntity.AddComponent(new Trigger(TriggerLayerType.FRIEND, new Vector2(-3f, 0f), 3f, 2f, 0.2f));
                EnemyAI enemyAI = enemyEntity.GetComponent<EnemyAI>();
                attackEntity.AddComponent(new Damage(enemyAI.Damage));
                Manager.GetInstance().AddEntity(attackEntity);

                // Spawn Attack Animation
                IEntity attackAnimationEntity = ParticleBuilder.StationaryParticle(enemyTransform.Position - new Vector2(2f, 0f), 0.2f, 0.5f, false, "Animations.slime_attack_left.png", 0.05f);
                Manager.GetInstance().AddEntity(attackAnimationEntity);
            }

            if (IsPlayerOnRight(direction))
            {
                // Spawn Attack Trigger
                IEntity attackEntity;
                attackEntity = new Entity(new Transform(enemyTransform.Position, 1f, 0), "AttackRight");
                attackEntity.AddComponent(new Trigger(TriggerLayerType.FRIEND, new Vector2(1f, 0f), 3f, 2f, 0.2f));
                EnemyAI enemyAI = enemyEntity.GetComponent<EnemyAI>();
                attackEntity.AddComponent(new Damage(enemyAI.Damage));
                Manager.GetInstance().AddEntity(attackEntity);

                // Spawn Attack Animation
                IEntity attackAnimationEntity = ParticleBuilder.StationaryParticle(enemyTransform.Position + new Vector2(1f, 0f), 0.2f, 0.5f, false, "Animations.slime_attack_right.png", 0.05f);
                Manager.GetInstance().AddEntity(attackAnimationEntity);
            }
        }

        private void Jump(IEntity enemyEntity, Vector2 direction)
        {
            Effect effect = enemyEntity.GetComponent<Effect>();
            if (effect.Effects.ContainsKey("JumpCooldown"))
                return;

            // Do the Jumpy
            Physics physics = enemyEntity.GetComponent<Physics>();
            physics.Acceleration = new Vector2(physics.Acceleration.X - (direction.X * 50f), physics.Acceleration.Y);
            physics.Velocity = new Vector2(physics.Velocity.X - (direction.X * 2.5f), Constants.JUMPSTRENGTH);

            effect.Effects.Add("JumpCooldown", 1);
        }
    }
}
