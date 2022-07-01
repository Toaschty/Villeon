using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.Utils;

namespace Villeon.EntityManagement
{
    public class EnemyBuilder
    {
        [Obsolete]
        public void Spawn(Vector2 position)
        {
            IEntity entity = new Entity(new Transform(position, 0.5f, 0f), "Peter");
            entity.AddComponent(new DynamicCollider(new Vector2(0f, 0f), position, 2f, 2f));
            entity.AddComponent(new Trigger(TriggerLayerType.ENEMY | TriggerLayerType.LADDER, new Vector2(0), 2f, 2f));
            entity.AddComponent(new Health(500));
            entity.AddComponent(new Effect());
            entity.AddComponent(new Physics());
            entity.AddComponent(new EnemyAI(20));
            entity.AddComponent(new JumpingAI());
            entity.AddComponent(new Sprite(Assets.Asset.GetTexture("Sprites.Empty.png"), SpriteLayer.Foreground, true));
            Random random = new Random();
            Color4 color = new Color4(1.0f, 0.5f, 0.2f, 1.0f);
            entity.AddComponent(new Light(color, -6f, 1f, new Vector2(0f, 0f)));

            // Setup player animations
            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.slime_jumping.png", 0.1f));
            entity.AddComponent(animController);
            Manager.GetInstance().AddEntity(entity);
        }

        public IEntity SlimeEntity(Vector2 position)
        {
            IEntity entity = new Entity(new Transform(position, 0.5f, 0f), "Slime");
            entity.AddComponent(new DynamicCollider(new Vector2(0f, 0f), position, 2f, 2f));
            entity.AddComponent(new Trigger(TriggerLayerType.ENEMY | TriggerLayerType.LADDER, new Vector2(0), 2f, 2f));
            entity.AddComponent(new Health(500));
            entity.AddComponent(new Effect());
            entity.AddComponent(new Physics());
            entity.AddComponent(new EnemyAI(20));
            entity.AddComponent(new JumpingAI());
            Sprite sprite = Assets.Asset.GetSpriteSheet("Animations.slime_jumping.png").GetSprite(0, SpriteLayer.Foreground, true);
            entity.AddComponent(sprite);
            Random random = new Random();
            Color4 color = new Color4(1.0f, 0.5f, 0.2f, 1.0f);
            entity.AddComponent(new Light(color, -6f, 1f, new Vector2(sprite.Width * 0.5f / 2f, sprite.Height * 0.5f / 2f)));

            // Setup player animations
            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.slime_jumping.png", 0.1f));
            entity.AddComponent(animController);

            return entity;
        }

        public IEntity BossSlimeEntity(Vector2 position)
        {
            IEntity entity = new Entity(new Transform(position, 5f, 0f), "Boss Slime");
            Sprite sprite = Assets.Asset.GetSpriteSheet("Animations.slime_jumping.png").GetSprite(0, SpriteLayer.Foreground, true);
            entity.AddComponent(sprite);
            entity.AddComponent(new DynamicCollider(new Vector2(sprite.Width * 5f / 2f, 0f), position, 2f, 2f));
            entity.AddComponent(new Trigger(TriggerLayerType.ENEMY | TriggerLayerType.LADDER, new Vector2(0), 2f, 2f));
            entity.AddComponent(new Health(500));
            entity.AddComponent(new Effect());
            entity.AddComponent(new Physics());
            entity.AddComponent(new EnemyAI(50));
            entity.AddComponent(new JumpingAI());
            Random random = new Random();
            Color4 color = new Color4(1.0f, 0.5f, 0.2f, 1.0f);
            entity.AddComponent(new Light(color, -3f, 5f, new Vector2(sprite.Width * 5f / 2f, sprite.Height * 5f / 2f)));

            // Setup player animations
            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.slime_jumping.png", 0.1f));
            entity.AddComponent(animController);

            return entity;
        }
    }
}
