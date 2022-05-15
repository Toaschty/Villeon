using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.ECS;

namespace Villeon.Render
{
    public class DynamicRenderBatch : RenderBatch
    {
        public DynamicRenderBatch(int maxBatchSize)
            : base(maxBatchSize)
        {
            _entities = new HashSet<IEntity>();
        }

        public void UpdateSprites()
        {
            foreach (IEntity entity in _entities)
            {
                AddEntity(entity);
            }
        }

        public override void Render()
        {
            Clear();
            UpdateSprites();
            LoadBuffer();
            base.Render();
        }
    }
}
