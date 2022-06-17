using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        private int _price;
        private int _itemStackSize;
        private ITEM_TYPE _itemType;

        public Item()
        {
            _name = "Null";
            _sprite = Asset.GetSprite("GUI.Items.EmptyItem.png", SpriteLayer.ScreenGuiMiddleground, true);
            _price = -1;
            _itemType = ITEM_TYPE.POTION;
        }

        public Item(string name, string sprite, int itemMaxStack, int price, ITEM_TYPE type)
        {
            _name = name;
            _sprite = Asset.GetSprite("GUI.Items." + sprite, SpriteLayer.ScreenGuiMiddleground, true);
            _price = price;
            _itemStackSize = itemMaxStack;
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

        public int StackSize
        {
            get { return _itemStackSize; }
        }
    }
}
