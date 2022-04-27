using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.Helper;

namespace Villeon.Systems
{
    internal class TileRenderSystem : IRenderSystem
    {
        private int _currentTexture = -1;

        public TileRenderSystem(string name, TileMap tileMap)
        {
            Name = name;
            TileMap = tileMap;
            Signature.Add(ComponentFlag.TILE);
        }

        public string Name { get; }

        public List<IEntity> Entities { get; } = new ();

        public Signature Signature { get; private set; } = new ();

        public TileMap TileMap { get; }

        public void Render()
        {
            _currentTexture = -1;
            foreach (var entity in Entities)
            {
                Tile tile = entity.GetComponent<Tile>();

                // Bind texure defined in tile
                if (_currentTexture != tile.TileSet.Texture2D)
                {
                    _currentTexture = tile.TileSet.Texture2D;
                    GL.BindTexture(TextureTarget.Texture2D, tile.TileSet.Texture2D);
                }

                Graphics.DrawTile(tile.WorldCoords, tile.TexCoords);
            }
        }
    }
}
