using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.Systems.Update
{
    public class PlayerFightingSystem : System, IUpdateSystem
    {
        public PlayerFightingSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Physics), typeof(Collider), typeof(Player), typeof(Effect));
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
                // Go to next player if not attacking
                if (!KeyHandler.IsPressed(Keys.Space))
                    continue;

                // Check if the attack is on cooldown
                Effect effect = player.GetComponent<Effect>()!;
                if (effect.Effects.ContainsKey("AttackCooldown"))
                    continue;

                int damage = 50;

                Player playerComponent = player.GetComponent<Player>();
                Transform transform = player.GetComponent<Transform>();

                // Spawn attack to left side of player
                if (playerComponent.WasLookingLeft)
                {
                    IEntity attackEntity;
                    attackEntity = new Entity(transform, "AttackLeft");
                    attackEntity.AddComponent(new Trigger(TriggerLayerType.ENEMY, new Vector2(-3f, 0f), 3f, 2f, 0.2f));
                    Sprite sprite = Asset.GetSprite("Sprites.Empty.png", SpriteLayer.GUIForeground, true);
                    sprite.Offset = new Vector2(-2, 0);

                    // Setup attack animation
                    AnimationController animController = new AnimationController();
                    animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.slash_attack_left.png", 0.05f));
                    attackEntity.AddComponent(animController);
                    attackEntity.AddComponent(sprite);
                    attackEntity.AddComponent(new Damage(damage));
                    Manager.GetInstance().AddEntity(attackEntity);
                }

                // Spawn attack to right side of player
                if (playerComponent.WasLookingRight)
                {
                    IEntity attackEntity;
                    attackEntity = new Entity(transform, "AttackRight");
                    attackEntity.AddComponent(new Trigger(TriggerLayerType.ENEMY, new Vector2(1f, 0f), 3f, 2f, 0.2f));
                    Sprite sprite = Asset.GetSprite("Sprites.Empty.png", SpriteLayer.GUIForeground, true);
                    sprite.Offset = new Vector2(1, 0);

                    // Setup attack animation
                    AnimationController animController = new AnimationController();
                    animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.slash_attack_right.png", 0.05f));
                    attackEntity.AddComponent(animController);
                    attackEntity.AddComponent(sprite);
                    attackEntity.AddComponent(new Damage(damage));
                    Manager.GetInstance().AddEntity(attackEntity);
                }

                // Add the attack Cooldown
                effect.Effects.Add("AttackCooldown", 0.1f);
            }
        }
    }
}
