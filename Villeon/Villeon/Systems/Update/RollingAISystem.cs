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
    public class RollingAISystem : System, IUpdateSystem
    {
        private IEntity? _playerEntity;

        public RollingAISystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Physics), typeof(DynamicCollider), typeof(RollingAI), typeof(EnemyAI), typeof(Effect))
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
                Transform enemyTransfrom = enemyEntity.GetComponent<Transform>();
                Effect effect = enemyEntity.GetComponent<Effect>();
                DynamicCollider enemyCollider = enemyEntity.GetComponent<DynamicCollider>();

                Vector2 distance = playerTransform.Position - enemyTransfrom.Position;
                float direction = 0;
                if (distance.X != 0)
                    direction = distance.X / Math.Abs(distance.X);

                Physics physics = enemyEntity.GetComponent<Physics>();
                physics.Acceleration += new Vector2(Constants.MOVEMENTSPEED * 0.3f * direction, physics.Acceleration.Y);


                if (distance.X < 20)
                {
                    if (distance.X < 2 && !effect.Effects.ContainsKey("AttackCooldown"))
                    {
                        IEntity attackEntity = new Entity(new Transform(enemyTransfrom.Position, 1f, 0), "Attack");
                        attackEntity.AddComponent(new Trigger(TriggerLayerType.FRIEND, Vector2.Zero, 1.0f, 1.0f, 0.1f));
                        EnemyAI enemyAI = enemyEntity.GetComponent<EnemyAI>();
                        attackEntity.AddComponent(new Damage(enemyAI.Damage));
                        Manager.GetInstance().AddEntity(attackEntity);
                        effect.Effects.Add("AttackCooldown", 0.5f);
                    }
                }
            }
        }
    }
}
