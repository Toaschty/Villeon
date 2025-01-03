﻿using System;
using Newtonsoft.Json;
using Villeon.Assets;
using Villeon.Components;

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

            string spriteName = itemJson!.sprite;
            Sprite sprite = Asset.GetSprite("GUI.Items." + spriteName, SpriteLayer.ScreenGuiMiddleground, true);
            int stackSize = itemJson.stackSize;
            Item.ITEM_TYPE type = itemJson.itemType;

            // Get dmg and defense values
            if (type == Item.ITEM_TYPE.WEAPON)
            {
                int damage = itemJson.damage;
                int defense = itemJson.defense;
                return new Item(name!, sprite, stackSize, type, sprite.RenderLayer, damage, defense);
            }

            return new Item(name!, sprite, stackSize, type, sprite.RenderLayer);
        }

        public static int GetHealthEffect(string potionName)
        {
            // If special potion was used -> Heal everything
            if (potionName == "HealthPotionSpecial")
                return 0;

            // Search for potion in json
            for (int i = 0; i < _itemsCount; i++)
            {
                if (_itemJson[i].name == potionName)
                {
                    // Return healing effect
                    return _itemJson[i].healEffect;
                }
            }

            return -1;
        }
    }
}
