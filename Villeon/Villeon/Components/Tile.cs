using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Tile : IComponent
    {
        public readonly float _x;
        public readonly float _y;

        private Vector2 _position;

        public TileSetStruct TileSet;

        public struct TileSetStruct
        {
            public int Texture2D { get; set; }
            public float TileWidth { get; set; }
            public float TileHeight { get; set; }
        }

        public Box2 TexCoords => new Box2(_x, _y, _x + TileSet.TileWidth, _y + TileSet.TileHeight);
        public Box2 WorldCoords => new Box2(_position, _position + new Vector2(1, 1));

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
            _position = position;
        }
    }
}
