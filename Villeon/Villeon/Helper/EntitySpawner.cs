using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon
{
    public class EntitySpawner
    {
        public EntitySpawner()
        {
        }

        public void Spawn(Vector2 position)
        {
            IEntity entity = new Entity(new Transform(position, new Vector2(0.5f, 1.0f), 0f), "Peter");
            entity.AddComponent(new Collider(Vector2.Zero, position, 0.5f, 1.0f));
            entity.AddComponent(new Physics());
            entity.AddComponent(new SimpleAI());
            entity.AddComponent(new Sprite(Color4.White, Assets.GetTexture("HenksFont.png"), RenderLayer.Front, true));
            Manager.GetInstance().AddEntity(entity);
        }
    }
}
