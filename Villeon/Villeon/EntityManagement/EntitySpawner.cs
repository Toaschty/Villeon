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
    public class EntitySpawner
    {
        public void Spawn(Vector2 position)
        {
            IEntity entity = new Entity(new Transform(position, 0.5f, 0f), "Peter");
            entity.AddComponent(new Collider(new Vector2(0f, 0f), position, 2f, 2f));
            entity.AddComponent(new DynamicCollider(entity.GetComponent<Collider>()));
            entity.AddComponent(new Trigger(TriggerLayerType.ENEMY | TriggerLayerType.LADDER, new Vector2(0), 2f, 2f));
            entity.AddComponent(new Health(500));
            entity.AddComponent(new Effect());
            entity.AddComponent(new Physics());
            entity.AddComponent(new EnemyAI());
            entity.AddComponent(new Sprite(Assets.Asset.GetTexture("Sprites.Empty.png"), SpriteLayer.Foreground, true));
            Random random = new Random();
            Color4 color = new Color4(1.0f, 0.5f, 0.2f, 1.0f);
            entity.AddComponent(new Light(color, -6f, 1f));

            // Setup player animations
            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.slime_jumping.png", 0.1f));
            entity.AddComponent(animController);
            Manager.GetInstance().AddEntity(entity);
        }
    }
}
