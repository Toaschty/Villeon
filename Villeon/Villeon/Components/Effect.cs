using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Effect : IComponent
    {
        public Dictionary<string, float> Effects { get; set; } = new Dictionary<string, float>();
    }
}
