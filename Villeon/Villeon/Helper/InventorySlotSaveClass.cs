using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;

namespace Villeon.Helper
{
    [Serializable]
    public class InventorySlotSaveClass
    {
        private string? _item = null;
        private int _count;

        public string Item
        {
            get { return _item!; }
            set { _item = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }
    }
}
