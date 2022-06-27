using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Exp : IComponent
    {
        private int _experience;

        public Exp(int exp)
        {
            _experience = exp;
        }

        public int Experience => _experience;
    }
}
