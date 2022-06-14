using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Zenseless.OpenTK;

namespace Villeon.Components
{
    public class Tile
    {
        // Texture coordinates of tile texture
        private readonly float _texCoordX;
        private readonly float _texCoordY;

        // Frameduration of animation if needed
        private float _frameDuration;

        public Tile(float textureCoordinateX, float textureCoordinateY, TileSetStruct tileset)
        {
            // Set texture coordinates
            _texCoordX = textureCoordinateX;
            _texCoordY = textureCoordinateY;

            // Set Tileset
            TileSet = tileset;

            // Set empty animation => Static tile
            FrameDuration = 0;
            AnimationFrames = new List<Tile>();
        }

        public float FrameDuration
        {
            get { return _frameDuration; }
            set { _frameDuration = value; }
        }

        public TileSetStruct TileSet { get; set; }

        public List<Tile> AnimationFrames { get; set; }

        public List<Box2> Colliders { get; set; } = new List<Box2>();

        public Box2 TexCoords => new Box2(_texCoordX, _texCoordY, _texCoordX + TileSet.TileWidth, _texCoordY + TileSet.TileHeight);

        public struct TileSetStruct
        {
            public Texture2D Texture2D { get; set; }

            public float TileWidth { get; set; }

            public float TileHeight { get; set; }

            public Vector2 Delta { get; set; }
        }
    }
}
