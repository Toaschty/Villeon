using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Render
{
    public interface ILayer
    {
        public void AddRenderingData(RenderingData data);

        public void Remove(RenderingData data);

        public void Render();
    }
}
