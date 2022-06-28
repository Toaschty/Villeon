using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class NPC : IComponent
    {
        public NPC(string name, string scene)
        {
            Name = name;
            Scene = scene;
        }

        public string Name { get; set; }

        public string Scene { get; set; }
    }
}
