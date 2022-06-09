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
            Signature.IncludeAND(typeof(Sprite));

            // Create World Layers
            for (int i = (int)SpriteLayer.Background; i <= (int)SpriteLayer.Collider; i++)
            {
                _spriteLayers[i] = new Layer(true);
            }

            // Create Screen Layers
            for (int i = (int)SpriteLayer.ScreenGuiBackground; i <= (int)SpriteLayer.ScreenGuiForeground; i++)
            {
                _spriteLayers[i] = new Layer(false);
            }

            _entityRenderData = new Dictionary<IEntity, List<RenderingData>>();
            _renderColliders = renderColliders;
        }

        public override void AddEntity(IEntity entity)
        {
            // Create new renderingDataList
            List<RenderingData> renderingDataList = new List<RenderingData>();

            // Add sprite to the appropriate Sprite Layer
            Sprite? sprite = entity.GetComponent<Sprite>();
            if (sprite is not null)
            {
                RenderingData data = AddSprite(entity);
                renderingDataList.Add(data);
            }

            // Get Collider add it to collider layer
            if (_renderColliders)
            {
                Trigger? trigger = entity.GetComponent<Trigger>();
                if (trigger is not null)
                {
                    RenderingData data = AddTriggerSprite(entity);
                    renderingDataList.Add(data);
                }

                Collider? collider = entity.GetComponent<Collider>();
                if (collider is not null)
                {
                    RenderingData data = AddColliderSprite(entity);
                    renderingDataList.Add(data);
                }
            }

            if (renderingDataList.Count != 0)
                _entityRenderData.Add(entity, renderingDataList);
        }

        public override void RemoveEntity(IEntity entity)
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

        private RenderingData AddSprite(IEntity entity)
        {
            Sprite? sprite = entity.GetComponent<Sprite>();
            Transform transform = entity.GetComponent<Transform>()!;
            RenderingData data = new RenderingData(sprite, transform, Vector2.Zero, transform.Scale);

            SpriteLayer spriteLayer = data.Sprite!.RenderLayer;
            _spriteLayers[(int)spriteLayer].AddRenderingData(data);
            return data;
        }

        private RenderingData AddTriggerSprite(IEntity entity)
        {
            Transform transform = entity.GetComponent<Transform>()!;
            Trigger? trigger = entity.GetComponent<Trigger>();

            Color4 color = new Color4(1f, 0f, 1f, 0.3f);
            Sprite sprite = new Sprite(SpriteLayer.Collider, trigger.Width, trigger.Height, true);
            sprite.Color = color;

            RenderingData data = new RenderingData(sprite, transform, trigger.Offset, Vector2.One);
            _spriteLayers[(int)SpriteLayer.Collider].AddRenderingData(data);
            return data;
        }

        private RenderingData AddColliderSprite(IEntity entity)
        {
            Transform transform = entity.GetComponent<Transform>()!;
            Collider? collider = entity.GetComponent<Collider>();

            Color4 color = new Color4(1f, 1f, 0f, 0.3f);
            Sprite sprite = new Sprite(SpriteLayer.Collider, collider.Width, collider.Height, true);
            sprite.Color = color;
            RenderingData data = new RenderingData(sprite, transform, collider.Offset, Vector2.One);
            _spriteLayers[(int)SpriteLayer.Collider].AddRenderingData(data);
            return data;
        }
    }
}
