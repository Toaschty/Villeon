using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Villeon.Helper;

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
            Graphics.DrawPolygon(Color4.White, CollisionSystem.CreatePolygon(collider));
            Graphics.DrawColliderQuad(Color4.Blue, collider.LastPosition, collider.Width, collider.Height);
            Graphics.DrawColliderQuad(Color4.Red, collider.Position, collider.Width, collider.Height);
        }
    }
}
