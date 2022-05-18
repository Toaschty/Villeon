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

        public void AddRenderingData(ref RenderingData data)
        {
            bool added = false;
            if (data.Sprite.IsDynamic == true)
            {
                added = AddDynamic(data);

                // Batch is full or not created: create new, add to batch!
                if (!added)
                {
                    RenderBatch newBatch = new RenderBatch();
                    newBatch.Start();
                    _dynamicBatches.Add(newBatch);
                    newBatch.AddSprite(data);
                }
            }
            else
            {
                added = AddStatic(data);

                // Batch is full or not created: create new, add to batch!
                if (!added)
                {
                    RenderBatch newBatch = new RenderBatch();
                    newBatch.Start();
                    _staticBatches.Add(newBatch);
                    newBatch.AddSprite(data);
                    newBatch.LoadBuffer();
                }
            }
        }

        public void Remove(RenderingData data)
        {
            foreach (RenderBatch dynamicBatch in _dynamicBatches)
            {
                dynamicBatch.RemoveEntity(data);
            }

            foreach (RenderBatch staticBatch in _staticBatches)
            {
                staticBatch.RemoveEntity(data);
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

        private bool AddDynamic(RenderingData data)
        {
            foreach (RenderBatch batch in _dynamicBatches)
            {
                if (!batch.Full())
                {
                    // Check for Texture batching
                    Texture2D texture = data.Sprite.Texture;
                    if (texture == null || (batch.HasTexture(texture) || batch.HasTextureRoom()))
                    {
                        batch.AddSprite(data);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool AddStatic(RenderingData data)
        {
            // Add the entity to the batch
            foreach (RenderBatch batch in _staticBatches)
            {
                if (!batch.Full())
                {
                    // Check for Texture batching
                    Texture2D texture = data.Sprite.Texture;
                    if (texture == null || (batch.HasTexture(texture) || batch.HasTextureRoom()))
                    {
                        batch.AddSprite(data);
                        batch.LoadBuffer();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
