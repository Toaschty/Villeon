using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.ECS;
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

        public HashSet<IEntity> Entities { get; private set; } = new ();

        public SystemSignature Signature { get; protected set; } = new ();
    }
}
