using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;
using Villeon.Systems;
using Zenseless.OpenTK;

namespace Villeon.Render
{
    [Obsolete]
    public class StaticSpriteRenderer : Villeon.Systems.System, IRenderSystem
    {
        private List<RenderBatch> _renderBatches = new List<RenderBatch>();
        private Dictionary<IEntity, RenderingData> _entityToRenderData = new Dictionary<IEntity, RenderingData>();

        private List<RenderingData> _newRenderingData = new List<RenderingData>();
        private List<IEntity> _removedEntities = new List<IEntity>();

        public StaticSpriteRenderer(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Sprite), typeof(Transform));

            _renderBatches.Add(new RenderBatch(Assets.GetShader("Shaders.shader")));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            Sprite sprite = entity.GetComponent<Sprite>();
            if (!sprite.IsDynamic)
            {
                // Add to RenderBatch
                Transform transform = entity.GetComponent<Transform>();
                RenderingData renderingData = new RenderingData(sprite, transform, Vector2.Zero, transform.Scale);
                _newRenderingData.Add(renderingData);
                _entityToRenderData.Add(entity, renderingData);
            }
        }

        public override void RemoveEntity(IEntity entity)
        {
            base.RemoveEntity(entity);
            _removedEntities.Add(entity);
        }

        public bool Contains(IEntity entity)
        {
            return _entityToRenderData.ContainsKey(entity);
        }

        public void Render()
        {
            RemoveDeletedEntities();
            AddNewRenderingData();
            foreach (RenderBatch renderBatch in _renderBatches)
            {
                renderBatch.Render();
            }
        }

        private void RemoveDeletedEntities()
        {
            // Is there Entities to Remove?
            if (_removedEntities.Count > 0)
            {
                // Remove all deleted RenderData from the batches
                foreach (IEntity removedEntity in _removedEntities)
                {
                    foreach (RenderBatch batch in _renderBatches)
                    {
                        batch.RemoveEntity(_entityToRenderData[removedEntity]);
                    }

                    _entityToRenderData.Remove(removedEntity);
                }

                // Refresh bufferdata on the gpu
                foreach (RenderBatch renderBatch in _renderBatches)
                {
                    renderBatch.Rebuffer();
                }

                // Get ready for the next Entities that could be removed
                _removedEntities.Clear();
            }
        }

        private void AddNewRenderingData()
        {
            if (_newRenderingData.Count > 0)
            {
                // Add Renderingdata, the Vertices into the vertexArray
                foreach (RenderingData data in _newRenderingData)
                {
                    AddRenderingData(data);
                }

                _newRenderingData.Clear();

                // Load bufferdata to the gpu
                foreach (RenderBatch renderBatch in _renderBatches)
                {
                    renderBatch.LoadBuffer();
                }
            }
        }

        private void AddRenderingData(RenderingData data)
        {
            bool added = false;
            added = AddStatic(data);

            // Batch is full or not created: create new, add to batch!
            if (!added)
            {
                RenderBatch newBatch = new RenderBatch(Assets.GetShader("Shaders.shader"));
                _renderBatches.Add(newBatch);
                newBatch.AddRenderingData(data);
                newBatch.AddSprite(data);
            }
        }

        private bool AddStatic(RenderingData data)
        {
            // Add the entity to the batch
            foreach (RenderBatch batch in _renderBatches)
            {
                if (!batch.Full())
                {
                    // Check for Texture batching
                    Texture2D texture = data.Sprite!.Texture!;
                    if (texture == null || (batch.HasTexture(texture) || batch.HasTextureRoom()))
                    {
                        batch.AddRenderingData(data);
                        batch.AddSprite(data);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
