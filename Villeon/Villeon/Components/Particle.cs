using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Particle : IComponent
    {
        public Particle(float ttl)
        {
            TTL = ttl;
            MaxTTL = ttl;
        }

        public float TTL { get; set; }

        public float MaxTTL { get; private set; }

        public bool IsFading { get; set; }

    }
}
