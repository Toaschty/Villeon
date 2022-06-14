using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Assets;
using Villeon.Components;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.GUI
{
    public class Item
    {
        private string _name;
        private Sprite _sprite;
        private int _prize;
        private ITEM_TYPE _itemType;

        public Item()
        {
            _name = "Empty";
            _sprite = Asset.GetSprite("GUI.Inventory.InventoryEmptySlot.png", SpriteLayer.GUIForeground, false);
            _prize = -1;
            _itemType = 0;
        }

        public Item(string name, Sprite sprite, int prize, ITEM_TYPE type)
        {
            _name = name;
            _sprite = sprite;
            _prize = prize;
            _itemType = type;
        }

        public enum ITEM_TYPE
        {
            WEAPON,
            POTION,
            MATERIAL,
        }

        public ITEM_TYPE ItemType
        {
            get { return _itemType; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Sprite Sprite
        {
            get { return _sprite; }
        }
    }
}
