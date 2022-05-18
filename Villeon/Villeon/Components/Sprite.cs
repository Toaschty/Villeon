using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Render;
using Zenseless.OpenTK;

namespace Villeon.Components
{
    public class Sprite : IComponent
    {
        private Texture2D? _texture2D;
        private SpriteLayer _layer;
        private Vector2[] _texCoords;
        private Color4 _color;
        private bool _isDynamic = false;
        private float _aspectRatio = 1f;
        private float _width = 1f;
        private float _height = 1f;

        // Create a sprite with Texture
        public Sprite(Color4 color, Texture2D texture, SpriteLayer renderLayer, Vector2[] texCoords, bool isDynamic = false)
        {
            _texture2D = texture;
            _layer = renderLayer;
            _texCoords = texCoords;
            _color = color;
            _isDynamic = isDynamic;
            _aspectRatio = texture.Height / (float)texture.Width;
        }

        public Sprite(Color4 color, Texture2D texture, SpriteLayer renderLayer, bool isDynamic = false)
        {
            _texture2D = texture;
            _layer = renderLayer;
            _texCoords = _texCoords = new Vector2[4]
            {
                new Vector2(0f, 0f),    // UV: Bottom left
                new Vector2(1f, 0f),    // UV: Bottom right
                new Vector2(0f, 1f),    // UV: Top left
                new Vector2(1f, 1f),    // UV: Top right
            };
            _color = color;
            _isDynamic = isDynamic;
            _aspectRatio = texture.Height / (float)texture.Width;
        }

        // Create a sprite with no Texture (Just color)
        public Sprite(Color4 color, SpriteLayer renderLayer, bool isDynamic = false)
        {
            _texture2D = null;
            _layer = renderLayer;
            _texCoords = new Vector2[4]
            {
                new Vector2(0f, 0f),    // UV: Bottom left
                new Vector2(1f, 0f),    // UV: Bottom right
                new Vector2(0f, 1f),    // UV: Top left
                new Vector2(1f, 1f),    // UV: Top right
            };
            _color = color;
            _isDynamic = isDynamic;
        }

        public void SetOwnDimensions(float width, float height)
        {
            _width = width;
            _height = height;
            _aspectRatio = 1f;
        }

        public Vector2[] TexCoords
        {
            get { return _texCoords; }
            set { _texCoords = value; }
        }

        public Texture2D? Texture
        {
            get { return _texture2D; }
            set { _texture2D = value; }
        }

        public Color4 Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public bool IsDynamic
        {
            get { return _isDynamic; }
            set { _isDynamic = value; }
        }

        public SpriteLayer RenderLayer
        {
            get { return _layer; }
            set { _layer = value; }
        }

        public float AspectRatio
        {
            get { return _aspectRatio; }
        }

        public float Width
        {
            get => _width;
            set => _width = value;
        }

        public float Height
        {
            get => _height;
            set => _height = value;
        }
    }
}
