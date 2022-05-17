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
        private List<RenderBatch> _dynamicBatches = new List<RenderBatch>();
        private List<RenderBatch> _staticBatches = new List<RenderBatch>();

        public void Add(Sprite sprite, Transform transform)
        {
            bool added = false;
            if (sprite.IsDynamic == true)
            {
                added = AddDynamic(sprite, transform);

                // Batch is full or not created: create new, add to batch!
                if (!added)
                {
                    RenderBatch newBatch = new RenderBatch();
                    newBatch.Start();
                    _dynamicBatches.Add(newBatch);
                    newBatch.AddSprite(sprite, transform);
                }
            }
            else
            {
                added = AddStatic(sprite, transform);

                // Batch is full or not created: create new, add to batch!
                if (!added)
                {
                    RenderBatch newBatch = new RenderBatch();
                    newBatch.Start();
                    _staticBatches.Add(newBatch);
                    newBatch.AddSprite(sprite, transform);
                    newBatch.LoadBuffer();
                }
            }
        }

        public void Remove(IEntity entity)
        {
            foreach (RenderBatch dynamicBatch in _dynamicBatches)
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

            foreach (RenderBatch renderBatch in _dynamicBatches)
            {
                renderBatch.RebufferAll();
                renderBatch.Render();
            }
        }

        private bool AddDynamic(Sprite sprite, Transform transform)
        {
            foreach (RenderBatch batch in _dynamicBatches)
            {
                if (!batch.Full())
                {
                    // Check for Texture batching
                    Texture2D texture = sprite.Texture;
                    if (texture == null || (batch.HasTexture(texture) || batch.HasTextureRoom()))
                    {
                        batch.AddSprite(sprite, transform);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool AddStatic(Sprite sprite, Transform transform)
        {
            // Add the entity to the batch
            foreach (RenderBatch batch in _staticBatches)
            {
                if (!batch.Full())
                {
                    // Check for Texture batching
                    Texture2D texture = sprite.Texture;
                    if (texture == null || (batch.HasTexture(texture) || batch.HasTextureRoom()))
                    {
                        batch.AddSprite(sprite, transform);
                        batch.LoadBuffer();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
