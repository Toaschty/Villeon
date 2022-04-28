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
    internal class TileRenderSystem : System, IRenderSystem
    {
        private int _currentTexture = -1;

        public TileRenderSystem(string name, TileMap tileMap)
            : base(name)
        {
            TileMap = tileMap;
            Signature = Signature.AddToSignature(typeof(Tile));
        }

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
