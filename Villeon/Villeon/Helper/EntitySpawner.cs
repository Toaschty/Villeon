using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
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
            IEntity entity = new Entity("Peter");
            entity.AddComponent(new Collider(Vector2.Zero, position, 2.0f, 2.0f));
            entity.AddComponent(new Transform(position, 1.0f, 1.0f));
            entity.AddComponent(new Physics());
            Manager.GetInstance().AddEntity(entity);
        }
    }
}
