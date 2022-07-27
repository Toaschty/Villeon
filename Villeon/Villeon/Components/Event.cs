using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Event : IComponent
    {
        public Event(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
