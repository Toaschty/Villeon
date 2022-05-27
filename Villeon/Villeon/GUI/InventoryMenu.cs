using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;
using Villeon.Render;

namespace Villeon.GUI
{
    public class InventoryMenu : IGUIMenu
    {
        private static InventoryMenu? _inventory;

        private List<IEntity> _allEntities = new List<IEntity>();
        private IEntity _backgroundImage;
        private InventorySlot[,] _inventorySlots;

        // Inventory
        private int _inventorySlotsX = 7;
        private int _inventorySlotsY = 4;

        // Align inventory slots
        private Vector2 _startPos = new Vector2(-5f, 1f);
        private float _slotScalingFactor = 0.3f;
        private float _slotSize = InventorySlot.SlotSize;
        private float _offset = 0.3f;

        // Scroll
        private float _scrollScale = 0.5f;

        private Vector2i _playerCurrentSlotPosition = new Vector2i(0, 0);

        public InventoryMenu()
        {
            _backgroundImage = CreateInventoryBackground();
            _inventorySlots = new InventorySlot[_inventorySlotsY, _inventorySlotsX];

            SetInventorySlotPositions();

            // Set the selection frame to the starting point
            _allEntities.Add(_inventorySlots[_playerCurrentSlotPosition.Y, _playerCurrentSlotPosition.X].SlotSelection);
            _allEntities.AddRange(GetAllSlotEntities()); // Add all slot entities
            _allEntities.Add(_backgroundImage); // Add the background sroll
        }

        public static InventoryMenu GetInstance()
        {
            if (_inventory == null)
                _inventory = new InventoryMenu();
            return _inventory;
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
                        _allEntities.Add(_inventorySlots[y, x].ItemEntity);
                        return; // New item has been added
                    }
                }
            }

            Console.WriteLine("Inventory FULL!");
        }

        public IEntity[] GetEntities()
        {
            return _allEntities.ToArray();
        }

        public void OnKeyReleased(Keys key)
        {
            // Remove the old selection frame
            _allEntities.Remove(_inventorySlots[_playerCurrentSlotPosition.Y, _playerCurrentSlotPosition.X].SlotSelection);
            Manager.GetInstance().RemoveEntity(_inventorySlots[_playerCurrentSlotPosition.Y, _playerCurrentSlotPosition.X].SlotSelection);

            if (key == Keys.H)
            {
                AddItem(new Item("HealthPotion", Assets.GetSprite("GUI.Potion_Item.png", Render.SpriteLayer.ScreenGuiForeground, false), 12, Item.Type.POTION));
                Console.WriteLine("Spawning Health potion!");
            }
            else if (key == Keys.W)
            {
                if (_playerCurrentSlotPosition.Y > 0)
                    _playerCurrentSlotPosition.Y -= 1;
            }
            else if (key == Keys.S)
            {
                if (_playerCurrentSlotPosition.Y < _inventorySlotsY - 1)
                    _playerCurrentSlotPosition.Y += 1;
            }
            else if (key == Keys.A)
            {
                if (_playerCurrentSlotPosition.X > 0)
                    _playerCurrentSlotPosition.X -= 1;
            }
            else if (key == Keys.D)
            {
                if (_playerCurrentSlotPosition.X < _inventorySlotsX - 1)
                    _playerCurrentSlotPosition.X += 1;
            }

            // Add the new selection frame
            _allEntities.Add(_inventorySlots[_playerCurrentSlotPosition.Y, _playerCurrentSlotPosition.X].SlotSelection);
        }

        private List<IEntity> GetAllSlotEntities()
        {
            List<IEntity> allSlotEntities = new List<IEntity>();

            // Get all inventory slot background entities
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    allSlotEntities.Add(_inventorySlots[y, x].SlotBackground);
                }
            }

            return allSlotEntities;
        }

        // Calculate the position for every slot
        private void SetInventorySlotPositions()
        {
            Vector2 position = _startPos;

            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    _inventorySlots[y, x] = new InventorySlot(new Transform(position, _slotScalingFactor, 0));

                    // calculate the next position
                    position += new Vector2(_offset + (_slotSize * _slotScalingFactor), 0);
                }

                // calculate the position for the new row
                position.X = _startPos.X;
                position.Y = position.Y - _offset - (_slotSize * _slotScalingFactor);
            }
        }

        private IEntity CreateInventoryBackground()
        {
            Sprite scrollImage = Assets.GetSprite("GUI.Scroll.png", SpriteLayer.ScreenGuiBackground, false);
            Vector2 middle = new Vector2(scrollImage.Width / 2f, scrollImage.Height / 2f);
            middle *= _scrollScale;

            IEntity background = new Entity(new Transform(new Vector2(0f) - middle, _scrollScale, 0f), "InventoryBackImage");
            background.AddComponent(scrollImage);

            return background;
        }
    }
}
