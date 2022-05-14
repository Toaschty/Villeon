using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Helper;
using Villeon.ECS;

namespace Villeon.Systems
{
    public interface ISystem
    {
        string Name { get; }

        HashSet<IEntity> Entities { get; }

        ulong Signature { get; }
    }
}
