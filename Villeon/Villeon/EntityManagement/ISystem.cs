using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Helper;

namespace Villeon.EntityManagement
{
    public interface ISystem
    {
        string Name { get; }

        HashSet<IEntity> Entities { get; }

        public SystemSignature Signature { get; }

        public void AddEntity(IEntity entity);

        public void RemoveEntity(IEntity entity);
    }
}
