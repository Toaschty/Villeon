using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Enemy : IComponent
    {
        public Enemy(string nameInJson)
        {
            Name = nameInJson;
        }

        public string Name { get; set; }
    }
}
