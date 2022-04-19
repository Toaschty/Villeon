using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Systems
{
    public interface ISystem : IUpdate
    {
        string Name { get; }

        List<Entity> Entities { get; }

        Signature Signature { get; }
    }
}
