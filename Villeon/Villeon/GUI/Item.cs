using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;

namespace Villeon.GUI
{
    public class Item
    {
        private string _name;
        private Sprite _sprite;
        private int _prize;
        private Type _itemType;

        public Item()
        {
            _name = "Empty";
            _sprite = Assets.GetSprite("GUI.Inventory.InventoryEmptySlot.png", Render.SpriteLayer.GUIForeground, false);
            _prize = -1;
            _itemType = 0;
        }

        public Item(string name, Sprite sprite, int prize, Type type)
        {
            _name = name;
            _sprite = sprite;
            _prize = prize;
            _itemType = type;
        }

        public enum Type
        {
            WEAPON,
            POTION,
            MATERIAL,
        }

        public Sprite Sprite
        {
            get { return _sprite; }
        }
    }
}
