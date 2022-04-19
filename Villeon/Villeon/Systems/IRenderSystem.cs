using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Systems
{
    public interface IRenderSystem
    {
        string Name { get; }

        void Render();
    }
}
