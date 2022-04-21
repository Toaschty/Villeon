using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Collider : IComponent
    {
        public Collider(Box2 bounds)
        {
            Bounds = bounds;
        }

        public Box2 Bounds { get; set; }
    }
}
