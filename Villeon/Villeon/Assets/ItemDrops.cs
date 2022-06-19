using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Villeon.GUI;

namespace Villeon.Assets
{
    public class ItemDrops
    {
        private static Dictionary<string, Item> _items = new Dictionary<string, Item>();

        public static Item GetItem(string itemName)
        {
            return _items[itemName];
        }

        public static void SetupDrops()
        {
            dynamic items = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.Items.json"))!;

            // Get ITEM inventory data
            for (int i = 0; i < items.Count; i++)
            {
                string name = items[i].name;
                int price = items[i].price;
                string itemTypeString = items[i].itemType;
                Item.ITEM_TYPE itemType = (Item.ITEM_TYPE)Enum.Parse(typeof(Item.ITEM_TYPE), itemTypeString, true);
                int stackSize = items[i].stackSize;
                string spriteName = items[i].sprite;

                _items.Add(name, new Item(name, spriteName, stackSize, price, itemType, Components.SpriteLayer.GUIBackground));
            }
        }
    }
}
