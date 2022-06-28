using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Components
{
    public class Physics : IComponent
    {
        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public Vector2 Acceleration { get; set; } = Vector2.Zero;

        public float Weight { get; set; } = 1.0f;

        public float Friction { get; set; } = 1.0f;
    }
}
