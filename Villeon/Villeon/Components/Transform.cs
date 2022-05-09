using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Components
{
    public class Transform : IComponent
    {
        public Transform(Vector2 position, float scale, float degrees)
        {
            Position = position;
            Scale = scale;
            Degrees = degrees;
        }

        public Vector2 Position { get; set; }

        public float Scale { get; set; }

        public float Degrees { get; set; }
    }
}
