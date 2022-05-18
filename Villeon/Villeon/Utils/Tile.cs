using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Zenseless.OpenTK;

namespace Villeon.Utils
{
    public class Tile
    {
        private readonly float _texCoordX;
        private readonly float _texCoordY;

        public Tile(float textureCoordinateX, float textureCoordinateY, TileSetStruct tileset)
        {
            _texCoordX = textureCoordinateX;
            _texCoordY = textureCoordinateY;
            TileSet = tileset;
        }

        public Tile(float textureCoordinateX, float textureCoordinateY, TileSetStruct tileset, List<Box2> colliders)
        {
            _texCoordX = textureCoordinateX;
            _texCoordY = textureCoordinateY;
            TileSet = tileset;
            Colliders = colliders;
        }

        public TileSetStruct TileSet { get; set; }

        public List<Box2> Colliders { get; set; } = new List<Box2>();

        public Box2 TexCoords => new Box2(_texCoordX, _texCoordY, _texCoordX + TileSet.TileWidth, _texCoordY + TileSet.TileHeight);

        public Sprite ToSprite()
        {
            Vector2[] texCoords = new Vector2[4]
            {
                new Vector2(TexCoords.Min.X, TexCoords.Min.Y),
                new Vector2(TexCoords.Max.X, TexCoords.Min.Y),
                new Vector2(TexCoords.Min.X, TexCoords.Max.Y),
                new Vector2(TexCoords.Max.X, TexCoords.Max.Y),
            };
            return new Sprite(Color4.White, TileSet.Texture2D, Render.SpriteLayer.Background, texCoords);
        }

        public struct TileSetStruct
        {
            public Texture2D Texture2D { get; set; }

            public float TileWidth { get; set; }

            public float TileHeight { get; set; }

            public Vector2 Delta { get; set; }
        }
    }
}
