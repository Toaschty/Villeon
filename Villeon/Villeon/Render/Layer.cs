using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Systems;
using Zenseless.OpenTK;

namespace Villeon.Render
{
    public class Layer : IRender
    {
        private List<DynamicRenderBatch> _dynamicBatches = new List<DynamicRenderBatch>();
        private List<RenderBatch> _staticBatches = new List<RenderBatch>();

        public void Add(IEntity entity, int maxBatchSize)
        {
            bool added = false;
            if (entity.GetComponent<Sprite>().IsDynamic == true)
            {
                // Dynamic Entity
                foreach (DynamicRenderBatch batch in _dynamicBatches)
                {
                    if (!batch.Full())
                    {
                        // Check for Texture batching
                        Texture2D texture = entity.GetComponent<Sprite>().Texture;
                        if (texture == null || (batch.HasTexture(texture) || batch.HasTextureRoom()))
                        {
                            batch.AddEntity(entity);
                            added = true;
                            break;
                        }
                    }
                }

                // Batch is full or not created: create new, add to batch!
                if (!added)
                {
                    DynamicRenderBatch newBatch = new DynamicRenderBatch(maxBatchSize);
                    newBatch.Start();
                    _dynamicBatches.Add(newBatch);
                    newBatch.AddEntity(entity);
                }
            }
            else
            {
                // Add the entity to the batch
                foreach (RenderBatch batch in _staticBatches)
                {
                    if (!batch.Full())
                    {
                        // Check for Texture batching
                        Texture2D texture = entity.GetComponent<Sprite>().Texture;
                        if (texture == null || (batch.HasTexture(texture) || batch.HasTextureRoom()))
                        {
                            batch.AddEntity(entity);
                            batch.LoadBuffer();
                            added = true;
                            break;
                        }
                    }
                }

                // Batch is full or not created: create new, add to batch!
                if (!added)
                {
                    RenderBatch newBatch = new RenderBatch(maxBatchSize);
                    newBatch.Start();
                    _staticBatches.Add(newBatch);
                    newBatch.AddEntity(entity);
                    newBatch.LoadBuffer();
                }
            }

        }

        public void Remove(IEntity entity)
        {
            foreach (DynamicRenderBatch dynamicBatch in _dynamicBatches)
            {
                dynamicBatch.RemoveEntity(entity);
            }

            foreach (RenderBatch staticBatch in _staticBatches)
            {
                staticBatch.RemoveEntity(entity);
            }
        }

        public void Render()
        {
            foreach (RenderBatch renderBatch in _staticBatches)
            {
                renderBatch.Render();
            }

            foreach (DynamicRenderBatch renderBatch in _dynamicBatches)
            {
                renderBatch.Render();
            }
        }
    }
}
