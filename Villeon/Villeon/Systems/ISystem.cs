using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.Systems
{
    public interface ISystem
    {
        string Name { get; }

        HashSet<IEntity> Entities { get; }

        public SystemSignature Signature { get; }
    }
}
