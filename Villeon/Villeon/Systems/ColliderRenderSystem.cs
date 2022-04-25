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
    public class ColliderRenderSystem : IRenderSystem
    {
        public ColliderRenderSystem(string name)
        {
            Name = name;
            Signature.Add<Collider>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; } = new();

        public Signature Signature { get; private set; } = new();

        public void Render()
        {
            foreach (IEntity entity in Entities)
            {
                DrawCollider(entity.GetComponent<Collider>());
            }
        }

        private void DrawCollider(Collider collider)
        {
            DrawPolygon(Color4.White, CollisionSystem.CreatePolygon(collider));
            DrawQuad(Color4.Red, collider.Position, collider.Width, collider.Height);
            DrawQuad(Color4.Blue, collider.LastPosition, collider.Width, collider.Height);
        }

        private void DrawQuad(Color4 color, Vector2 point, float width, float height)
        {
            GL.Color4(color);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(point);
            GL.Vertex2(point.X + width, point.Y);
            GL.Vertex2(point.X + width, point.Y + height);
            GL.Vertex2(point.X, point.Y + height);
            GL.End();

            color.A = 0.3f;
            GL.Color4(color);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(point);
            GL.Vertex2(point.X + width, point.Y);
            GL.Vertex2(point.X + width, point.Y + height);
            GL.Vertex2(point.X, point.Y + height);
            GL.End();
        }

        private void DrawPolygon(Color4 color, List<Vector2> points)
        {
            GL.Color4(color);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Begin(PrimitiveType.LineLoop);
            foreach (Vector2 point in points)
            {
                GL.Vertex2(point);
            }
            GL.End();
        }
    }
}
