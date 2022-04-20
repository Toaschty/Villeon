using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Villeon.Components;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Villeon.Systems
{
    public class SpriteRenderSystem : IRenderSystem
    {
        public SpriteRenderSystem(string name)
        {
            Name = name;
            Signature.Add<SpriteDrawable>();
            Signature.Add<Transform>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; } = new();

        public Signature Signature { get; private set; } = new();

        public void Render()
        {
            // Renders all the Sprites
            foreach (IEntity entity in Entities)
            {
                SpriteDrawable sprite = entity.GetComponents<SpriteDrawable>().First();
                Transform transform = entity.GetComponents<Transform>().First();
                Vector2 position = transform.Position;

                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(position.X, position.Y);
                GL.Vertex2(position.X + sprite.Size.X, position.Y);
                GL.Vertex2(position + sprite.Size);
                GL.Vertex2(position.X, position.Y + sprite.Size.Y);
                GL.End();
                
                Console.WriteLine("Rendering color: " + sprite.Color);

            }
        }
    }
}
