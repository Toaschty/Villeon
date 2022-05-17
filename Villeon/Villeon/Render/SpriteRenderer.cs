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

        public SpriteRenderer(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Sprite));

            // Init Sprite Layers
            for (int i = 0; i < _numSpriteLayers; i++)
            {
                _spriteLayers[i] = new Layer();
            }
        }

        public void Add(IEntity entity)
        {
            Sprite sprite = entity.GetComponent<Sprite>();
            Transform transform = entity.GetComponent<Transform>();
            Collider collider = entity.GetComponent<Collider>();
            SpriteLayer spriteLayer = sprite.RenderLayer;

            //// Add sprite to the appropriate Sprite Layer
            //if (sprite is not null)
            _spriteLayers[(int)spriteLayer].Add(sprite, transform);

            // Get Collider add it to collider layer
            if (collider is not null)
            {
                Color4 color = new Color4(1f, 1f, 0f, 0.3f);
                Sprite colliderSprite = new Sprite(color, SpriteLayer.Collider, true);
                _spriteLayers[(int)SpriteLayer.Collider].Add(colliderSprite, transform);
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
