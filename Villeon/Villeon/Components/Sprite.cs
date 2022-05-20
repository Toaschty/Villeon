﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Render;
using Villeon.Utils;
using Zenseless.OpenTK;

namespace Villeon.Components
{
    public class Sprite : IComponent
    {
        private Color4 _color = Color4.White;
        private Texture2D? _texture2D;
        private SpriteLayer _layer;
        private Vector2[] _texCoords;
        private bool _isDynamic = false;
        private float _width = 1f;
        private float _height = 1f;
        private float _aspectRatio = 1f;
        private float _tilePixels = 8;

        // Create a sprite from a Spritesheet
        public Sprite(Texture2D texture, SpriteLayer renderLayer, Vector2[] texCoords, float spriteWidth, float spriteHeight, bool isDynamic = false)
        {
            _texture2D = texture;
            _layer = renderLayer;
            _texCoords = texCoords;
            _isDynamic = isDynamic;
            _width = spriteWidth / _tilePixels;
            _height = spriteHeight / _tilePixels;
            _aspectRatio = _height / _width;
        }

        // Sprite has the full texture as size
        public Sprite(Texture2D texture, SpriteLayer renderLayer, bool isDynamic = false)
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
            _isDynamic = isDynamic;
            _width = _texture2D.Width / _tilePixels;
            _height = _texture2D.Height / _tilePixels;
            _aspectRatio = _height / _width;
        }

        // Create a sprite with no Texture, using tile widths
        public Sprite(SpriteLayer renderLayer, float tileWidth, float tileHeight, bool isDynamic = false)
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
            _isDynamic = isDynamic;
            _width = tileWidth;
            _height = tileHeight;
            _aspectRatio = _height / _width;
        }

        // Create sprite from tile
        public Sprite(Tile tile, Box2 tileTexCoords)
        {
            Vector2[] texCoords = new Vector2[4]
            {
                new Vector2(tileTexCoords.Min.X, tileTexCoords.Min.Y),
                new Vector2(tileTexCoords.Max.X, tileTexCoords.Min.Y),
                new Vector2(tileTexCoords.Min.X, tileTexCoords.Max.Y),
                new Vector2(tileTexCoords.Max.X, tileTexCoords.Max.Y),
            };
            _texture2D = tile.TileSet.Texture2D;
            _layer = Render.SpriteLayer.Background;
            _texCoords = texCoords;
            _isDynamic = false;
            _width = 1f;
            _height = 1f;
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
