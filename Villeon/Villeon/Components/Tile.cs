using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Zenseless.OpenTK;

namespace Villeon.Components
{
    public class Tile : IComponent, ICloneable
    {
        public readonly float _x;
        public readonly float _y;

        public Tile(float x, float y, TileSetStruct tileset, Texture2D texture2D)
        {
            _x = x;
            _y = y;
            TileSet = tileset;
            Texture2D = texture2D;
        }

        public Tile(float x, float y, TileSetStruct tileset, List<Box2> colliders, Texture2D texture2D)
        {
            _x = x;
            _y = y;
            TileSet = tileset;
            Colliders = colliders;
            Texture2D = texture2D;
        }

        public Vector2 Position { get; set; }

        public TileSetStruct TileSet { get; set; }

        public List<Box2> Colliders { get; set; } = new List<Box2>();

        public Box2 TexCoords => new Box2(_x, _y, _x + TileSet.TileWidth, _y + TileSet.TileHeight);

        public Box2 WorldCoords => new Box2(Position, Position + new Vector2(1, 1));

        public Texture2D Texture2D { get; set; }

        public virtual object Clone() => new Tile(_x, _y, TileSet, Colliders, Texture2D);

        public struct TileSetStruct
        {
            public int Texture2D { get; set; }

            public float TileWidth { get; set; }

            public float TileHeight { get; set; }

            public Vector2 Delta { get; set; }
        }

        public Sprite ToSprite(Tile currentTile)
        {
            Vector2[] texCoords = new Vector2[4]
            {
                new Vector2(currentTile.TexCoords.Min.X, currentTile.TexCoords.Min.Y),
                new Vector2(currentTile.TexCoords.Max.X, currentTile.TexCoords.Min.Y),
                new Vector2(currentTile.TexCoords.Min.X, currentTile.TexCoords.Max.Y),
                new Vector2(currentTile.TexCoords.Max.X, currentTile.TexCoords.Max.Y),
            };
            return new Sprite(Color4.White, currentTile.Texture2D, Render.SpriteLayer.Background, texCoords);
        }
    }
}
