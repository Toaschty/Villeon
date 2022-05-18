using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.ECS;

namespace Villeon.Systems
{
    public interface IRenderSystem : IRender, ISystem
    {
        void Remove(IEntity entity);

        void Add(IEntity entity);
    }
}
