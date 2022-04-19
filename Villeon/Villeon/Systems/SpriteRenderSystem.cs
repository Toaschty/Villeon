using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Systems
{
    public class SpriteRenderSystem : IRenderSystem
    {
        public SpriteRenderSystem(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void Render()
        {
            // Renders all the Sprites
        }
    }
}
