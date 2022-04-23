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
    public class RenderSystem : IRenderSystem
    {
        public RenderSystem(string name)
        {
            Name = name;
            Signature.Add<Collider>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; } = new();

        public Signature Signature { get; private set; } = new();

        private Matrix4 refCameraMatrix = Matrix4.Identity;

        public void Render()
        {
            Camera.Update(Entities.First());
            refCameraMatrix  = Camera.GetMatrix();
            GL.LoadMatrix(ref refCameraMatrix);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            foreach (IEntity entity in Entities)
            {
                DrawCollider(entity.GetComponent<Collider>());
            }
        }

        private void DrawCollider(Collider collider)
        {
            DrawQuad(Color4.White, collider.Position, collider.Width, collider.Height);
        }

        private void DrawQuad(Color4 color, Vector2 point, float width, float height)
        {
            GL.Color4(color);
            GL.Begin(BeginMode.LineLoop);
            GL.Vertex2(point);
            GL.Vertex2(point.X + width, point.Y);
            GL.Vertex2(point.X + width, point.Y + height);
            GL.Vertex2(point.X, point.Y + height);
            GL.End();
        }
    }
}
