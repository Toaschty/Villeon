using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class PlayerFightingSystem : System, IUpdateSystem
    {
        public PlayerFightingSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Physics), typeof(DynamicCollider), typeof(Player), typeof(Effect), typeof(Health));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
        }

        public override void RemoveEntity(IEntity entity)
        {
            base.RemoveEntity(entity);
        }

        public void Update(float time)
        {
            foreach (IEntity player in Entities)
            {
                player.GetComponent<Health>().Protection = Stats.GetInstance().GetDefense();

                // Go to next player if not attacking
                if (!(MouseHandler.IsMouseDown() || KeyHandler.IsPressed(Keys.O) || KeyHandler.IsPressed(Keys.Enter)))
                    continue;

                // Check if the attack is on cooldown
                Effect effect = player.GetComponent<Effect>() !;
                if (effect.Effects.ContainsKey("AttackCooldown"))
                    continue;

                int damage = Stats.GetInstance().GetAttack();

                Player playerComponent = player.GetComponent<Player>();
                Transform transform = player.GetComponent<Transform>();

                // Spawn attack to left side of player
                if (playerComponent.WasLookingLeft)
                {
                    // Spawn Attack Trigger
                    IEntity attackEntity;
                    attackEntity = new Entity(transform, "TriggerAttackLeft");
                    attackEntity.AddComponent(new Trigger(TriggerLayerType.ENEMY, new Vector2(-4f, 0f), 4f, 3f, 0.2f));
                    attackEntity.AddComponent(new Damage(damage));
                    Manager.GetInstance().AddEntity(attackEntity);

                    // Spawn Attack Animation
                    IEntity attackAnimationEntity = ParticleBuilder.AnimatedStationaryParticle(transform.Position - new Vector2(2f, 0f), 0.2f, 0.5f, false, "Animations.slash_attack_left.png", 0.05f, SpriteLayer.Middleground);
                    Manager.GetInstance().AddEntity(attackAnimationEntity);
                }

                // Spawn attack to right side of player
                if (playerComponent.WasLookingRight)
                {
                    // Spawn Attack Trigger
                    IEntity attackEntity;
                    attackEntity = new Entity(transform, "TriggerAttackRight");
                    attackEntity.AddComponent(new Trigger(TriggerLayerType.ENEMY, new Vector2(0f, 0f), 4f, 3f, 0.2f));
                    attackEntity.AddComponent(new Damage(damage));
                    Manager.GetInstance().AddEntity(attackEntity);

                    // Spawn Attack Animation
                    IEntity attackAnimationEntity = ParticleBuilder.AnimatedStationaryParticle(transform.Position + new Vector2(1f, 0f), 0.2f, 0.5f, false, "Animations.slash_attack_right.png", 0.05f, SpriteLayer.Middleground);
                    Manager.GetInstance().AddEntity(attackAnimationEntity);
                }

                // Add the attack Cooldown
                effect.Effects.Add("AttackCooldown", 0.1f);
            }
        }
    }
}
