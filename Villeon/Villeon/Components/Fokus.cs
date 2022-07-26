using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Components
{
    public class Fokus : IComponent
    {
        public Fokus()
        {
            Offset = Vector2.Zero;
        }

        public Fokus(Vector2 offset)
        {
            Offset = offset;
        }

        public Vector2 Offset { get; set; }

        public float Intensity { get; set; } = 100f;
    }
}
