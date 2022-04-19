using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class SpriteDrawable
    {
        public SpriteDrawable(Color4 color)
        {
            Color = color;
        }

        Color4 Color { get; set; }
        // IGraphic graphic
    }
}
