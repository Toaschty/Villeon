using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon
{
    public class World
    {
        public List<Entity> activeEntities;

        public World()
        {
            activeEntities = new List<Entity>();
        }

        public void Update()
        {
            foreach (var entity in activeEntities)
            {
                entity.Update();
            }
        }

        public void Draw(Renderer renderer)
        {
            foreach (var entity in activeEntities)
            {
                entity.Draw(renderer);
            }
        }

        public void AddEntity(Entity entity)
        {
            activeEntities.Add(entity);
        }
    }
}
