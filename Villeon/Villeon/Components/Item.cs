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
        private int _itemStackSize;
        private ITEM_TYPE _itemType;

        private int _damage = 0;
        private int _defense = 0;

        public Item()
        {
            _name = "Null";
            _sprite = new Sprite(SpriteLayer.ScreenGuiMiddleground, 0, 0, true);
            _itemType = ITEM_TYPE.POTION;
        }

        public Item(string name, Sprite sprite, int itemMaxStack, ITEM_TYPE type)
        {
            _name = name;
            _sprite = sprite;
            _itemStackSize = itemMaxStack;
            _itemType = type;
        }

        public Item(string name, Sprite sprite, int itemMaxStack, ITEM_TYPE type, SpriteLayer layer)
        {
            _name = name;
            _sprite = sprite;
            _sprite.RenderLayer = layer;
            _itemStackSize = itemMaxStack;
            _itemType = type;
        }

        public Item(string name, Sprite sprite, int itemMaxStack, ITEM_TYPE type, SpriteLayer layer, int damage, int defense)
        {
            _name = name;
            _sprite = sprite;
            _sprite.RenderLayer = layer;
            _itemStackSize = itemMaxStack;
            _itemType = type;
            _damage = damage;
            _defense = defense;
        }

        public Item(Item copy)
        {
            _name = copy.Name;
            _sprite = new Sprite(copy.Sprite);
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

        public int Damage => _damage;

        public int Defense => _defense;
    }
}
