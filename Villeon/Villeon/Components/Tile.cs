using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Components
{
    public class Tile : IComponent, ICloneable
    {
        private readonly float _x;
        private readonly float _y;

        public Tile(float x, float y, TileSetStruct tileset)
        {
            _x = x;
            _y = y;
            TileSet = tileset;
        }

        public Tile(Vector2 position, float x, float y, TileSetStruct tileset)
        {
            _x = x;
            _y = y;
            TileSet = tileset;
            Position = position;
        }

        public Tile(float x, float y, TileSetStruct tileset, List<Box2> colliders)
        {
            _x = x;
            _y = y;
            TileSet = tileset;
            Colliders = colliders;
        }

        public Vector2 Position { get; set; }

        public TileSetStruct TileSet { get; set; }

        public List<Box2> Colliders { get; set; } = new List<Box2>();

        public Box2 TexCoords => new Box2(_x + 0.0001f, _y + 0.0001f, _x + TileSet.TileWidth - 0.0001f, _y + TileSet.TileHeight - 0.0001f);

        public Box2 WorldCoords => new Box2(Position, Position + new Vector2(1, 1));

        public object Clone()
        {
            return new Tile(_x, _y, TileSet, Colliders);
        }

        public struct TileSetStruct
        {
            public int Texture2D { get; set; }

            public float TileWidth { get; set; }

            public float TileHeight { get; set; }
        }
    }
}
