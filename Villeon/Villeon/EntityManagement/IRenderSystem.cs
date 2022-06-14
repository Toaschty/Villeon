using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.EntityManagement
{
    public interface IRenderSystem : IRender, ISystem
    {
        bool Contains(IEntity entity);
    }
}
