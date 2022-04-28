using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Helper;

namespace Villeon.Systems
{
    public class System : ISystem
    {
        public System(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public List<IEntity> Entities { get; private set; } = new ();

        public ulong Signature { get; protected set; } = 0;
    }
}
