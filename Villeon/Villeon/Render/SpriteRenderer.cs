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

            _renderColliders = renderColliders;
        }

        public void Add(IEntity entity)
        {
            Sprite? sprite = entity.GetComponent<Sprite>();
            Transform transform = entity.GetComponent<Transform>() !;
            Collider? collider = entity.GetComponent<Collider>();
            Trigger? trigger = entity.GetComponent<Trigger>();
            RenderingData data = default(RenderingData);
            data.Transform = transform;

            //// Add sprite to the appropriate Sprite Layer
            if (sprite is not null)
            {
                data.Sprite = sprite;
                data.Width = transform.Scale.X;
                data.Height = transform.Scale.Y;
                SpriteLayer spriteLayer = sprite.RenderLayer;
                _spriteLayers[(int)spriteLayer].Add(data);
            }

            // Get Collider add it to collider layer
            if (_renderColliders)
            {
                if (collider is not null)
                {
                    Color4 color = new Color4(1f, 1f, 0f, 0.3f);
                    data.Sprite = new Sprite(color, SpriteLayer.Collider, true);
                    data.Width = collider.Width;
                    data.Height = collider.Height;
                    _spriteLayers[(int)SpriteLayer.Collider].Add(data);
                }

                if (trigger is not null)
                {
                    Color4 color = new Color4(1f, 0f, 0f, 0.3f);
                    data.Sprite = new Sprite(color, SpriteLayer.Collider, true);
                    data.Width = trigger.Width;
                    data.Height = trigger.Height;
                    _spriteLayers[(int)SpriteLayer.Collider].Add(data);
                }
            }
        }

        public void Remove(IEntity entity)
        {
            for (int i = 0; i < _numSpriteLayers; i++)
            {
                _spriteLayers[i].Remove(entity);
            }
        }

        public void Render()
        {
            for (int i = 0; i < _numSpriteLayers; i++)
            {
                _spriteLayers[i].Render();
            }
        }
    }
}
