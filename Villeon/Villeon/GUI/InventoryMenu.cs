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
        private InventorySlot[,] _inventorySlots;
        private IEntity[,] _tabBar = new IEntity[2, 2];

        // Inventory
        private int _inventorySlotsX = 7;
        private int _inventorySlotsY = 4;

        // Align inventory slots
        private Vector2 _startPos = new Vector2(-5f, 0.3f);
        private float _slotScalingFactor = 0.3f;
        private float _slotSize = InventorySlot.SlotSize;
        private float _offset = 0.3f;
        private bool _onSlots = true;

        // Scroll
        private float _scrollScale = 0.5f;

        private Vector2i _playerInventoryPosition = new Vector2i(0, 0);
        private Vector2i _playerTabbarPosition = new Vector2i(0, 1);
        private Vector2i _activeTabbar = new Vector2i(0, 1);
        private Vector2i? _firstMovingPoint = null;

        private InventoryMenu()
        {
            _inventorySlots = new InventorySlot[_inventorySlotsY, _inventorySlotsX];

            SetInventorySlotPositions();
            InitializeTabbar();
            AddEntitiesToList();
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

        public bool OnKeyReleased(Keys key)
        {
            if (key == Keys.H)
            {
                AddItem(new Item("HealthPotion", Assets.GetSprite("GUI.Potion_Item.png", Render.SpriteLayer.ScreenGuiForeground, false), 12, Item.Type.POTION));
                Console.WriteLine("Spawning Health potion!");
            }

            if (_onSlots)
                HandleInventorySlot(key);
            else
                HandleTabNavigation(key);

            return true;
        }

        private void HandleInventorySlot(Keys key)
        {
            // Remove the old selection frame
            _allEntities.Remove(_inventorySlots[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);
            Manager.GetInstance().RemoveEntity(_inventorySlots[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);

            switch (key)
            {
                // Moving Up
                case Keys.W:
                    if (_playerInventoryPosition.Y > 0)
                    {
                        _playerInventoryPosition.Y -= 1;
                    }
                    else
                    {
                        // Reset the inventory slot background and add the navigation background
                        Manager.GetInstance().RemoveEntity(_inventorySlots[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);
                        _onSlots = false;
                        return;
                    }

                    break;

                // Moving Down
                case Keys.S:
                    if (_playerInventoryPosition.Y < _inventorySlotsY - 1)
                        _playerInventoryPosition.Y += 1;
                    else
                        _playerInventoryPosition.Y = 0;

                    break;

                // Moving Left
                case Keys.A:
                    if (_playerInventoryPosition.X > 0)
                        _playerInventoryPosition.X -= 1;
                    else
                        _playerInventoryPosition.X = _inventorySlotsX - 1;

                    break;

                // Moving Right
                case Keys.D:
                    if (_playerInventoryPosition.X < _inventorySlotsX - 1)
                        _playerInventoryPosition.X += 1;
                    else
                        _playerInventoryPosition.X = 0;

                    break;
                case Keys.Space:
                    // Swapping Items
                    if (_firstMovingPoint == null)
                    {
                        // First point not set
                        _firstMovingPoint = new Vector2i(_playerInventoryPosition.X, _playerInventoryPosition.Y);
                    }
                    else
                    {
                        Vector2i secondPoint = new Vector2i(_playerInventoryPosition.X, _playerInventoryPosition.Y);
                        SwapItems((Vector2i)_firstMovingPoint, secondPoint);
                    }

                    break;
            }

            // Add the new selection frame
            _allEntities.Add(_inventorySlots[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);
        }

        private void HandleTabNavigation(Keys key)
        {
            _allEntities.Remove(_tabBar[_playerTabbarPosition.Y, _playerTabbarPosition.X]);
            Manager.GetInstance().RemoveEntity(_tabBar[_playerTabbarPosition.Y, _playerTabbarPosition.X]);

            switch (key)
            {
                // Moving Up
                case Keys.W:
                    if (_playerTabbarPosition.Y < 1)
                        _playerTabbarPosition.Y += 1;

                    break;

                // Moving Down
                case Keys.S:
                    if (_playerTabbarPosition.Y > 0)
                    {
                        _playerTabbarPosition.Y -= 1;
                    }
                    else
                    {
                        // Set the active tabbar and add inventory slot background
                        _allEntities.Add(_inventorySlots[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);
                        _playerTabbarPosition = new Vector2i(_activeTabbar.X, _activeTabbar.Y); // reset the position to the active tabbar position
                        _onSlots = true;
                    }

                    break;

                // Moving Left
                case Keys.A:
                    if (_playerTabbarPosition.X > 0)
                        _playerTabbarPosition.X -= 1;
                    else
                        _playerTabbarPosition.X = 1;

                    break;

                // Moving Right
                case Keys.D:
                    if (_playerTabbarPosition.X < 1)
                        _playerTabbarPosition.X += 1;
                    else
                        _playerTabbarPosition.X = 0;
                    break;

                case Keys.Space:
                    // Player is changing the tab
                    _activeTabbar = new Vector2i(_playerTabbarPosition.Y, _playerTabbarPosition.X);
                    _allEntities.Add(_inventorySlots[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);
                    _onSlots = true;

                    break;
            }

            _allEntities.Add(_tabBar[_playerTabbarPosition.Y, _playerTabbarPosition.X]);
        }

        private void SwapItems(Vector2i firstPoint, Vector2i secondPoint)
        {
            Item? firstItem = _inventorySlots[firstPoint.Y, firstPoint.X].Item;
            Item? secondItem = _inventorySlots[secondPoint.Y, secondPoint.X].Item;

            if ((firstItem == null && secondItem == null) || firstPoint.Equals(secondPoint))
            {
                _firstMovingPoint = null; // Reset the first selected point
                return;
            }

            if (firstItem != null)
            {
                // Remove the old item entity
                _allEntities.Remove(_inventorySlots[firstPoint.Y, firstPoint.X].ItemEntity);

                // Remove the old item sprite from the inventory
                Manager.GetInstance().RemoveEntity(_inventorySlots[firstPoint.Y, firstPoint.X].ItemEntity);
            }

            if (secondItem != null)
            {
                // Remove the old item entity
                _allEntities.Remove(_inventorySlots[secondPoint.Y, secondPoint.X].ItemEntity);

                // Remove the old item sprite from the inventory
                Manager.GetInstance().RemoveEntity(_inventorySlots[secondPoint.Y, secondPoint.X].ItemEntity);
            }

            _inventorySlots[firstPoint.Y, firstPoint.X].Item = secondItem;
            _inventorySlots[secondPoint.Y, secondPoint.X].Item = firstItem;

            if (secondItem != null)
                _allEntities.Add(_inventorySlots[firstPoint.Y, firstPoint.X].ItemEntity);

            if (firstItem != null)
                _allEntities.Add(_inventorySlots[secondPoint.Y, secondPoint.X].ItemEntity);

            _firstMovingPoint = null; // Reset the first selected point
        }

        private void AddEntitiesToList()
        {
            _allEntities.AddRange(GetAllSlotEntities()); // Add all slot entities
            _allEntities.AddRange(GetTabbarEntities()); // Add the text that will be on the top
            _allEntities.Add(_inventorySlots[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection); // Set the selection frame to the starting point
            _allEntities.Add(CreateInventoryBackground()); // Add the background sroll
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

        private List<IEntity> GetTabbarEntities()
        {
            List<IEntity> tabBarEntities = new List<IEntity>();

            float letterSpacing = 0.2f;
            float lineSpacing = 0.5f;
            float letterScale = 0.4f;

            float positionY = _startPos.Y + 2f;

            // Make text
            Text allText = new Text("All", new Vector2(_startPos.X + 1.4f, positionY + 0.8f), "Alagard", letterSpacing, lineSpacing, letterScale);
            tabBarEntities.AddRange(allText.GetEntities());

            Text weaponText = new Text("Weapons", new Vector2(_startPos.X, positionY - 0.4f), "Alagard", letterSpacing, lineSpacing, letterScale);
            tabBarEntities.AddRange(weaponText.GetEntities());

            Text potionText = new Text("Potions", new Vector2(_startPos.X + 6.2f, positionY + 0.8f), "Alagard", letterSpacing, lineSpacing, letterScale);
            tabBarEntities.AddRange(potionText.GetEntities());

            Text materialText = new Text("Materials", new Vector2(_startPos.X + 5.7f, positionY - 0.4f), "Alagard", letterSpacing, lineSpacing, letterScale);
            tabBarEntities.AddRange(materialText.GetEntities());

            float horizontalLineY = _startPos.Y + 2.5f;
            float horizontalLineScale = 0.3f;

            // Make HirzontalLines
            IEntity firstHorizontalLine = new Entity(new Transform(new Vector2(_startPos.X + 0.2f, horizontalLineY), horizontalLineScale, 0f), "InventoryHorizontalLine");
            Sprite firstHorizontalSprite = Assets.GetSprite("GUI.Scroll_Horizontal_Line_1.png", SpriteLayer.ScreenGuiForeground, false);
            firstHorizontalLine.AddComponent(firstHorizontalSprite);
            tabBarEntities.Add(firstHorizontalLine);

            IEntity secondHorizontalLine = new Entity(new Transform(new Vector2(_startPos.X + 6f, horizontalLineY), horizontalLineScale, 0f), "InventoryHorizontalLine");
            Sprite secondHorizontalSprite = Assets.GetSprite("GUI.Scroll_Horizontal_Line_2.png", SpriteLayer.ScreenGuiForeground, false);
            secondHorizontalLine.AddComponent(secondHorizontalSprite);
            tabBarEntities.Add(secondHorizontalLine);

            // Set the first element as active
            tabBarEntities.Add(_tabBar[_playerTabbarPosition.Y, _playerTabbarPosition.X]);

            return tabBarEntities;
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

        private void InitializeTabbar()
        {
            float scale = 0.4f;
            float positionX = _startPos.X -0.3f;
            float positionY = _startPos.Y + 1.6f;
            float offsetX = 5.8f;
            float offsetY = 1.3f;

            _tabBar[0, 0] = new Entity(new Transform(new Vector2(positionX, positionY), scale, 0f), "Weapons");
            _tabBar[0, 1] = new Entity(new Transform(new Vector2(positionX + offsetX, positionY), scale, 0f), "Materials");
            _tabBar[1, 0] = new Entity(new Transform(new Vector2(positionX, positionY + offsetY), scale, 0f), "All");
            _tabBar[1, 1] = new Entity(new Transform(new Vector2(positionX + offsetX, positionY + offsetY), scale, 0f), "Potions");

            Sprite selectionBackground = Assets.GetSprite("GUI.Scroll_Selection.png", SpriteLayer.ScreenGuiMiddleground, false);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    _tabBar[i, j].AddComponent(selectionBackground);
                }
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