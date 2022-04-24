using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Sprite : IComponent
    {
        public Sprite(Color4 color, Vector2 size)
        {
            Color = color;
            Size = size;
        }

        public Color4 Color { get; set; }

        public Vector2 Size { get; set; }
    }
}
