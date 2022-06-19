using System;
using Newtonsoft.Json;
using Villeon.Assets;

namespace Villeon.GUI
{
    public static class ItemLoader
    {
        private static dynamic _itemJson;
        private static int _itemsCount;

        static ItemLoader()
        {
            _itemJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.Items.json")) !;
            _itemsCount = _itemJson.Count;
        }

        public static Item GetItem(int index)
        {
            if (index > _itemsCount)
                return new Item();

            dynamic itemJson = _itemJson[index];

            string name = itemJson.name.ToString();
            string sprite = itemJson.sprite.ToString();
            int price = itemJson.price;
            int stackSize = itemJson.stackSize;
            Item.ITEM_TYPE type = itemJson.itemType;

            return new Item(name, sprite, stackSize, price, type);
        }

        public static Item GetItem(string itemName)
        {
            dynamic? itemJson = null;
            string? name = null;
            for (int i = 0; i < _itemsCount; i++)
            {
                name = _itemJson[i].name;
                if (name.Equals(itemName))
                {
                    itemJson = _itemJson[i];
                    break;
                }
            }

            if (itemJson == null || name == null)
                return new Item();

            string sprite = itemJson!.sprite;
            int price = itemJson.price;
            int stackSize = itemJson.stackSize;
            Item.ITEM_TYPE type = itemJson.itemType;

            return new Item(name!, sprite, stackSize, price, type);
        }
    }
}
