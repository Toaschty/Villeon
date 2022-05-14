using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.Systems
{
    internal class AnimatedTileRenderSystem : System //IRenderSystem
    {
        private int _currentTexture = -1;

        public AnimatedTileRenderSystem(string name, TileMap tileMap)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(AnimatedTile));
            TileMap = tileMap;
        }

        public TileMap TileMap { get; }

        public void Render()
        {
            _currentTexture = -1;
            foreach (var entity in Entities)
            {
                AnimatedTile animTile = entity.GetComponent<AnimatedTile>();

                Tile tile = TileMap._tiles[(uint)animTile.AnimationFrames[animTile.CurrentFrame] + 1];

                // Bind texure defined in tile
                if (_currentTexture != tile.TileSet.Texture2D)
                {
                    _currentTexture = tile.TileSet.Texture2D;
                    GL.BindTexture(TextureTarget.Texture2D, tile.TileSet.Texture2D);
                }

                Graphics.DrawTile(animTile.WorldCoords, tile.TexCoords);
            }
        }
    }
}
