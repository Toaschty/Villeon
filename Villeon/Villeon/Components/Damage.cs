using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Damage : IComponent
    {
        public Damage(int amount)
        {
            Amount = amount;
        }

        public int Amount { get; set; }
    }
}
