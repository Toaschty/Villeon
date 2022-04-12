using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon
{
    public abstract class Entity
    {
        public abstract void Update();
        public abstract void Draw(Renderer renderer);
    }
}
