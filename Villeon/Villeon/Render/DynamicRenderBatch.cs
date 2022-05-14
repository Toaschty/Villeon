using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.ECS;

namespace Villeon.Render
{
    public class DynamicRenderBatch : StaticRenderBatch
    {
        private List<IEntity> _entities;

        public DynamicRenderBatch(int maxBatchSize)
            : base(maxBatchSize)
        {
            _entities = new List<IEntity>();
        }

        public void AddEntity(IEntity entity)
        {
            //base.AddSprite(entity);
            _entities.Add(entity);
        }

        public void RemoveEntity(IEntity entity)
        {
            _entities.Remove(entity);
        }

        public void UpdateSprites()
        {
            foreach (IEntity entity in _entities)
            {
                AddSprite(entity);
            }
        }

        public override void Render()
        {
            Clear();
            UpdateSprites();
            Load();
            base.Render();
        }
    }
}
