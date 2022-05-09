using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Villeon;
using Villeon.Components;

namespace Villeon.Systems
{
    public class SpriteRenderSystem : System, IRenderSystem
    {
        public SpriteRenderSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Sprite));
        }

        public void Render()
        {
        }
    }
}
