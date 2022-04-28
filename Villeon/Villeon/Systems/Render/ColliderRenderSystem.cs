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
    public class ColliderRenderSystem : System, IRenderSystem
    {
        public ColliderRenderSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Collider));
        }

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
