using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.Helper;
using OpenTK.Mathematics;

namespace Villeon
{
    public class EntitySpawner
    {
        private readonly Manager _manager;

        public EntitySpawner(Manager manager)
        {
            _manager = manager;
        }

        public void Spawn(Vector2 position)
        {
            IEntity entity = new Entity("Peter");
            entity.AddComponent(new Collider(position, 2.0f, 2.0f));
            entity.AddComponent(new Transform(position, 1.0f, 1.0f));
            entity.AddComponent(new Physics());
            _manager.AddEntity(entity);
        }
    }
}
