using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.ECS;
using Villeon.Systems;

namespace Villeon.Render
{
    public class OnScreenRenderer : Villeon.Systems.System, IRenderSystem
    {
        public OnScreenRenderer(string name) 
            : base(name)
        {
        }

        public void Add(IEntity entity)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            
        }
    }
}
