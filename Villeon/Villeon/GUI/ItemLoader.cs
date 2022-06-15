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
            _itemsCount = _itemJson.items.Count;
            Console.WriteLine("Length: " + _itemJson.items.Count);
        }

        public static Item GetItem(int index)
        {
            if (index > _itemsCount)
                return new Item();

            dynamic itemJson = _itemJson.items[index];

            string name = itemJson.name.ToString();
            string sprite = itemJson.sprite.ToString();
            int prize = itemJson.prize;
            Item.ITEM_TYPE type = itemJson.itemType;

            return new Item(name, sprite, prize, type);
        }

        public static Item GetItem(string itemName)
        {
            dynamic? itemJson = null;

            for (int i = 0; i < _itemsCount; i++)
            {
                if (_itemJson[i].name.ToString().equals(itemName))
                {
                    itemJson = _itemJson[i];
                    break;
                }
            }

            if (itemJson == null)
                return new Item();

            string name = itemJson.name.ToString();
            string sprite = itemJson.sprite.ToString();
            int prize = itemJson.prize;
            Item.ITEM_TYPE type = itemJson.itemType;

            return new Item(name, sprite, prize, type);
        }
    }
}
