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
    public class SimpleAISystem : System, IUpdateSystem
    {
        private IEntity? _playerEntity;

        public SimpleAISystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Physics), typeof(Collider), typeof(EnemyAI))
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
            foreach (IEntity enemyEntity in Entities)
            {
                Transform transform = enemyEntity.GetComponent<Transform>();
                Vector2 playerDirection = transform.Position - playerTransform.Position;

                float side = -(playerDirection.X / MathF.Abs(playerDirection.X));
                if (playerDirection.Length < 10 && playerDirection.Length > 2.2f)
                {
                    Physics physics = enemyEntity.GetComponent<Physics>();
                    Collider collider = enemyEntity.GetComponent<Collider>();

                    physics.Acceleration += new Vector2(Constants.MOVEMENTSPEED * side * 0.4f, physics.Acceleration.Y);

                    if (side < 0 && collider.HasCollidedLeft && collider.HasCollidedBottom)
                        physics.Velocity = new Vector2(physics.Velocity.X, Constants.JUMPSTRENGTH);
                    else if (side > 0 && collider.HasCollidedRight && collider.HasCollidedBottom)
                        physics.Velocity = new Vector2(physics.Velocity.X, Constants.JUMPSTRENGTH);
                }
                else if (playerDirection.Length <= 10)
                {
                    // Player is in range to attack!
                    Effect effect = enemyEntity.GetComponent<Effect>() !;
                    if (!effect.Effects.ContainsKey("AttackCooldown"))
                    {
                        //// Attacking is not on cooldown, so attack!
                        IEntity attackEntity;
                        Vector2 enemyPosition = transform.Position;
                        if (side == 1)
                        {
                            // Left attack
                            attackEntity = new Entity(new Transform(enemyPosition, 1f, 0), "AttackRight");
                            attackEntity.AddComponent(new Trigger(TriggerLayerType.FRIEND, new Vector2(2f, 0f), 2f, 2f, 0.1f));
                        }
                        else
                        {
                            // Right attack
                            attackEntity = new Entity(new Transform(enemyPosition, 1f, 0), "AttackLeft");
                            attackEntity.AddComponent(new Trigger(TriggerLayerType.FRIEND, new Vector2(-2f, 0f), 2f, 2f, 0.1f));
                        }

                        attackEntity.AddComponent(new Damage(10));
                        Manager.GetInstance().AddEntity(attackEntity);

                        effect.Effects.Add("AttackCooldown", 1);
                    }
                }
            }
        }

        //HashSet<IEntity> allEntities = Manager.GetInstance().GetEntities();
        //IEntity? player = null;
        //foreach (IEntity entity in allEntities)
        //{
        //    if (entity.GetComponent<Player>() != null)
        //    {
        //        player = entity;
        //    }
        //}
    }
}
