using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Villeon.Components;

namespace Villeon.Systems
{
    public class SpriteRenderSystem : IRenderSystem
    {
        public SpriteRenderSystem(string name)
        {
            Name = name;
            Signature.Add<Sprite>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; } = new ();

        public Signature Signature { get; } = new ();

        public void Render()
        {
        }
    }
}
