using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Item
    {
        private string _name;
        private Sprite _sprite;
        private int _prize;

        public Item(string name, Sprite sprite, int prize)
        {
            _name = name;
            _sprite = sprite;
            _prize = prize;
        }

        private enum Type
        {
            WEAPON,
            POTION,
            MATERIAL,
        }
    }
}
