using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.Systems
{
    internal class TriggerRenderSystem : System
    {
        public TriggerRenderSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Trigger));
        }

        public void Render()
        {
            foreach (IEntity entity in Entities)
            {
                Trigger trigger = entity.GetComponent<Trigger>();
                Graphics.DrawColliderQuad(Color4.Purple, trigger.Position, trigger.Width, trigger.Height);
            }
        }
    }
}
