using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Boss : IComponent
    {
        public Boss()
        {
        }

        public bool IsDead { get; private set; } = false;
    }
}
