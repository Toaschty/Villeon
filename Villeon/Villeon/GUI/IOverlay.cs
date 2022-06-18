using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.EntityManagement;

namespace Villeon.GUI
{
    internal interface IOverlay
    {
        public IEntity[] GetEntities();
    }
}
