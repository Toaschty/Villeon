using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Systems;

namespace Villeon.Render
{
    public class SpriteRenderer : Villeon.Systems.System, IRenderSystem
    {
        private static int _numSpriteLayers = Enum.GetNames(typeof(SpriteLayer)).Length;
        private Dictionary<IEntity, List<RenderingData>> _entityRenderData;
        private Layer[] _spriteLayers = new Layer[_numSpriteLayers];
        private bool _renderColliders = false;

        public SpriteRenderer(string name, bool renderColliders)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Transform));

            // Create Sprite Layers
            for (int i = 0; i < _numSpriteLayers; i++)
            {
                _spriteLayers[i] = new Layer();
            }

            _entityRenderData = new Dictionary<IEntity, List<RenderingData>>();
            _renderColliders = renderColliders;
        }

        public void Add(IEntity entity)
        {
            // Create new renderingDataList
            List<RenderingData> renderingDataList = new List<RenderingData>();

            Sprite? sprite = entity.GetComponent<Sprite>();
            Transform transform = entity.GetComponent<Transform>() !;
            Collider? collider = entity.GetComponent<Collider>();
            Trigger? trigger = entity.GetComponent<Trigger>();
            RenderingData data = default(RenderingData);
            data.Transform = transform;
            data.Scale = transform.Scale;
            data.Offset = new Vector2(0, 0);

            // Add sprite to the appropriate Sprite Layer
            if (sprite is not null)
            {
                AddSprite(sprite, ref data);
                renderingDataList.Add(data);
            }

            // Get Collider add it to collider layer
            if (_renderColliders)
            {
                if (trigger is not null)
                {

                    AddTriggerSprite(trigger, ref data);
                    renderingDataList.Add(data);
                }

                if (collider is not null)
                {
                    data.Offset = collider.Offset;
                    AddColliderSprite(collider, ref data);
                    renderingDataList.Add(data);
                }
            }

            if (renderingDataList.Count != 0)
                _entityRenderData.Add(entity, renderingDataList);
        }

        private void AddSprite(Sprite? sprite, ref RenderingData data)
        {
            data.Sprite = sprite;
            SpriteLayer spriteLayer = sprite.RenderLayer;
            _spriteLayers[(int)spriteLayer].AddRenderingData(ref data);
        }

        private void AddTriggerSprite(Trigger? trigger, ref RenderingData data)
        {
            Color4 color = new Color4(1f, 0f, 1f, 0.3f);
            data.Sprite = new Sprite(SpriteLayer.Collider, trigger.Width, trigger.Height, true);
            data.Sprite.Color = color;
            data.Offset = trigger.Offset;
            data.Scale = new Vector2(1f, 1f);
            _spriteLayers[(int)SpriteLayer.Collider].AddRenderingData(ref data);
        }

        private void AddColliderSprite(Collider? collider, ref RenderingData data)
        {
            Color4 color = new Color4(1f, 1f, 0f, 0.3f);
            data.Sprite = new Sprite(SpriteLayer.Collider, collider.Width, collider.Height, true);
            data.Sprite.Color = color;
            data.Offset = -collider.Offset;
            _spriteLayers[(int)SpriteLayer.Collider].AddRenderingData(ref data);
        }

        public void Remove(IEntity entity)
        {
            for (int i = 0; i < _numSpriteLayers; i++)
            {
                foreach (RenderingData data in _entityRenderData[entity])
                    _spriteLayers[i].Remove(data);
            }

            _entityRenderData.Remove(entity);
        }

        public void Render()
        {
            for (int i = 0; i < _numSpriteLayers; i++)
            {
                _spriteLayers[i].Render();
            }
        }

        public bool Contains(IEntity entity)
        {
            return _entityRenderData.ContainsKey(entity);
        }
    }
}
