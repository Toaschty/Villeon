using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;
using Villeon.Render;

namespace Villeon.GUI
{
    public class InventoryMenu : IGUIMenu
    {
        private static InventoryMenu? _inventory;

        private List<IEntity> _allEntities;
        private IEntity _backgroundImage;
        private InventorySlot[,] _inventorySlots;

        // Inventory
        private int _inventorySlotsX = 7;
        private int _inventorySlotsY = 4;

        // Align inventory slots
        private Vector2 _startPos = new Vector2(-0.46f, 0.2f);
        private float _scale = 0.0285f;
        private float _slotSize = 0.114f;
        private float _offset = 0.02f;

        IEntity _item;

        public InventoryMenu()
        {
            _allEntities = new List<IEntity>();
            _backgroundImage = CreateInventoryBackground();
            _inventorySlots = new InventorySlot[_inventorySlotsY, _inventorySlotsX];

            SetInventorySlotPositions();
        }

        public static InventoryMenu GetInstance()
        {
            if (_inventory == null)
                _inventory = new InventoryMenu();
            return _inventory;
        }

        public IEntity[] GetEntities()
        {
            List<IEntity> list = new List<IEntity>();

            // Get all inventory slot background entities
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    list.Add(_inventorySlots[y, x].SlotBackground);
                }
            }

            // Get all item entities
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    if (_inventorySlots[y, x].HasItem())
                    {
                        list.Add(_inventorySlots[y, x].ItemEntity);
                    }
                }
            }

            list.Add(_backgroundImage); // Add the background sroll
            return list.ToArray();
        }

        public void AddItem(Item newItem)
        {
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    if (_inventorySlots[y, x].HasItem() == false)
                    {
                        // There is no item in the current slot
                        _inventorySlots[y, x].Item = newItem;
                        return; // New item has been added
                    }
                }
            }
        }

        // Calculate the position for every slot
        private void SetInventorySlotPositions()
        {
            Vector2 position = _startPos;

            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    _inventorySlots[y, x] = new InventorySlot(new Transform(position, _scale, 0));

                    // calculate the next position
                    position += new Vector2(_offset + _slotSize, 0);
                }

                // calculate the position for the new row
                position.X = _startPos.X;
                position.Y = position.Y - _offset - (_slotSize * (16f / 9f));
            }
        }

        private IEntity CreateInventoryBackground()
        {
            IEntity background = new Entity(new Transform(new Vector2(-0.75f, -0.6f), 0.04f, 0f), "InventoryBackImage");
            Sprite scrollImage = Assets.GetSprite("GUI.Scroll.png", SpriteLayer.ScreenGuiBackground, false);
            background.AddComponent(scrollImage);

            return background;
        }
    }
}
