using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Physics : IComponent
    {
        public Physics(Vector2 velocity, Vector2 acceleration)
        {
            Velocity = velocity;
            Acceleration = acceleration;
        }

        Vector2 Velocity { get; set; }
        Vector2 Acceleration { get; set; }
    }
}
