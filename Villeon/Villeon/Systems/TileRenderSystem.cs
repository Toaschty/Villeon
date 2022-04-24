using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;

namespace Villeon.Systems
{
    internal class TileRenderSystem : IRenderSystem
    {
        public TileRenderSystem(string name, TileMap tileMap)
        {
            Name = name;
            TileMap = tileMap;
            Signature.Add<Tile>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; } = new();

        public Signature Signature { get; private set; } = new();

        public TileMap TileMap { get; }

        private Matrix4 refCameraMatrix = Matrix4.Identity;

        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Camera.Update(Entities.Last());
            refCameraMatrix = Camera.GetMatrix();
            GL.LoadMatrix(ref refCameraMatrix);

            foreach (var entity in Entities)
            {
                Tile tile = entity.GetComponent<Tile>();

                // Bind texure defined in tile
                GL.BindTexture(TextureTarget.Texture2D, tile.TileSet.Texture2D);
                Draw(tile.WorldCoords, tile.TexCoords);
            }
        }

        private void Draw(Box2 rectangle, Box2 texCoords)
        {
            GL.Color4(Color4.White);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(texCoords.Min);
            GL.Vertex2(rectangle.Min);
            GL.TexCoord2(texCoords.Max.X, texCoords.Min.Y);
            GL.Vertex2(rectangle.Max.X, rectangle.Min.Y);
            GL.TexCoord2(texCoords.Max);
            GL.Vertex2(rectangle.Max);
            GL.TexCoord2(texCoords.Min.X, texCoords.Max.Y);
            GL.Vertex2(rectangle.Min.X, rectangle.Max.Y);
            GL.End();
        }
    }
}
