using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Helper;

namespace Villeon.Systems
{
    public interface ISystem
    {
        string Name { get; }

        List<IEntity> Entities { get; }

        ulong Signature { get; }
    }
}
