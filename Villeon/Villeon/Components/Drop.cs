using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.GUI;

namespace Villeon.Components
{
    public class Drop : IComponent
    {
        public Drop(Item item)
        {
            Item = item;
        }

        public Item Item { get; }

    }
}
