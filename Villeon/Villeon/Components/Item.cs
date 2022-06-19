using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Villeon.Assets;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.Components
{
    public class Item
    {
        private string _name;
        private Sprite _sprite;
        private int _price;
        private int _itemStackSize;
        private ITEM_TYPE _itemType;

        public Item()
        {
            _name = "Null";
            _sprite = new Sprite(SpriteLayer.ScreenGuiMiddleground, 0, 0, true);
            _price = -1;
            _itemType = ITEM_TYPE.POTION;
        }

        public Item(string name, Sprite sprite, int itemMaxStack, int price, ITEM_TYPE type)
        {
            _name = name;
            _sprite = sprite;
            _price = price;
            _itemStackSize = itemMaxStack;
            _itemType = type;
        }

        public Item(string name, Sprite sprite, int itemMaxStack, int price, ITEM_TYPE type, SpriteLayer layer)
        {
            _name = name;
            _sprite = sprite;
            _sprite.RenderLayer = layer;
            _price = price;
            _itemStackSize = itemMaxStack;
            _itemType = type;
        }

        public Item(Item copy)
        {
            _name = copy.Name;
            _sprite = copy.Sprite;
            _price = copy.Price;
            _itemStackSize = copy.StackSize;
            _itemType = copy.ItemType;
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

        public int StackSize
        {
            get { return _itemStackSize; }
        }

        public int Price
        {
            get { return _price; }
        }
    }
}
