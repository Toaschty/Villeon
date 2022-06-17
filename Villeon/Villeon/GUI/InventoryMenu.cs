using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.GUI
{
    public class InventoryMenu : IGUIMenu
    {
        private static InventoryMenu? _inventory;

        private List<IEntity> _allEntities = new List<IEntity>();

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

        // Inventories
        private InventorySlot[,] _activeInventory;
        private InventorySlot[,] _allInventory;
        private InventorySlot[,] _weaponInventory;
        private InventorySlot[,] _potionInventory;
        private InventorySlot[,] _materialInventory;

        private InventoryMenu()
        {
            _activeInventory = new InventorySlot[_inventorySlotsY, _inventorySlotsX];
            _allInventory = new InventorySlot[_inventorySlotsY, _inventorySlotsX];
            _weaponInventory = new InventorySlot[_inventorySlotsY, _inventorySlotsX];
            _potionInventory = new InventorySlot[_inventorySlotsY, _inventorySlotsX];
            _materialInventory = new InventorySlot[_inventorySlotsY, _inventorySlotsX];

            SetSlotPositions();
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
            // Add Items to the specific Inventory it belongs
            switch (newItem.ItemType)
            {
                case Item.ITEM_TYPE.MATERIAL:
                    AddItemsToInventory(_materialInventory, newItem);
                    break;
                case Item.ITEM_TYPE.POTION:
                    AddItemsToInventory(_potionInventory, newItem);
                    break;
                case Item.ITEM_TYPE.WEAPON:
                    AddItemsToInventory(_weaponInventory, newItem);
                    break;
            }

            if (_activeInventory == _allInventory)
                SetupAllInventory();
        }

        public IEntity[] GetEntities()
        {
            return _allEntities.ToArray();
        }

        public bool OnKeyReleased(Keys key)
        {
            if (key == Keys.H)
            {
                AddItem(ItemLoader.GetItem("Sword"));
                Console.WriteLine("Spawning Health potion!");
            }
            else if (key == Keys.G)
            {
                AddItem(ItemLoader.GetItem("HealthPotion"));
                Console.WriteLine("Spawning Health potion!");
            }

            if (_onSlots)
                HandleInventorySlot(key);
            else
                HandleTabNavigation(key);

            return true;
        }

        private void SetSlotPositions()
        {
            SetInventorySlotPositions(_allInventory);
            SetInventorySlotPositions(_weaponInventory);
            SetInventorySlotPositions(_potionInventory);
            SetInventorySlotPositions(_materialInventory);
            _activeInventory = _allInventory;
        }

        private void SetupAllInventory()
        {
            List<Item> allItems = new List<Item>();

            // Clear the whole Inventory
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    _allInventory[y, x].Item = null;
                }
            }

            // Get all weapons
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    if (_weaponInventory[y, x].HasItem())
                        allItems.Add(_weaponInventory[y, x].Item!);
                }
            }

            // Get all potions
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    if (_potionInventory[y, x].HasItem())
                        allItems.Add(_potionInventory[y, x].Item!);
                }
            }

            // Get all materials
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    if (_materialInventory[y, x].HasItem())
                        allItems.Add(_materialInventory[y, x].Item!);
                }
            }

            // Set all Items into the Inventory
            foreach (Item item in allItems)
            {
                AddItemsToInventory(_allInventory, item);
            }
        }

        private void AddItemsToInventory(InventorySlot[,] inventory, Item newItem)
        {
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    if (inventory[y, x].HasItem() == false)
                    {
                        inventory[y, x].Item = newItem; // Adding new Item

                        if (inventory == _activeInventory)
                            _allEntities.Add(inventory[y, x].ItemEntity);
                        return;
                    }
                }
            }
        }

        private void HandleInventorySlot(Keys key)
        {
            // Remove the old selection frame
            _allEntities.Remove(_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);
            Manager.GetInstance().RemoveEntity(_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);

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
                        Manager.GetInstance().RemoveEntity(_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);
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
            _allEntities.Add(_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);

            if (_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].HasItem())
                Console.WriteLine("Item: " + _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Item!.Name);
            else
                Console.WriteLine("Item: Null");
        }

        public Item? GetCurrentlySelectedItem()
        {
            if (_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Item is not null)
            {
                Item? item = _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Item;
                return item;
            }

            return null;
        }

        private void UseItem(Item item)
        {
            switch (item.ItemType)
            {
                case Item.ITEM_TYPE.POTION:

                    break;
            }
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
                        // Remove the old tabbar Background
                        if (!_playerTabbarPosition.Equals(_activeTabbar))
                            Manager.GetInstance().RemoveEntity(_tabBar[_playerTabbarPosition.Y, _playerTabbarPosition.X]);

                        Console.WriteLine("ActiveTabbar: " + _activeTabbar + " || Player: " + _playerTabbarPosition);

                        // Add the slot background
                        _allEntities.Add(_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);
                        _playerTabbarPosition = new Vector2i(_activeTabbar.X, _activeTabbar.Y); // set the tabbar position to the active tabbar position
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
                    _activeTabbar = new Vector2i(_playerTabbarPosition.X, _playerTabbarPosition.Y);
                    _allEntities.Add(_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);
                    ChangeInventory(_activeTabbar);
                    _onSlots = true;

                    break;
            }

            _allEntities.Add(_tabBar[_playerTabbarPosition.Y, _playerTabbarPosition.X]);
        }

        private void ChangeInventory(Vector2i tabbarPosition)
        {
            IEntity currentTab = _tabBar[tabbarPosition.Y, tabbarPosition.X];

            RemoveInventoryEntities();

            switch (currentTab.Name)
            {
                case "All":
                    Console.WriteLine("All");
                    SetupAllInventory();
                    _activeInventory = _allInventory;
                    break;
                case "Materials":
                    Console.WriteLine("Materials");
                    _activeInventory = _materialInventory;
                    break;
                case "Potions":
                    Console.WriteLine("Potions");
                    _activeInventory = _potionInventory;
                    break;
                case "Weapons":
                    Console.WriteLine("Weapons");
                    _activeInventory = _weaponInventory;
                    break;
            }

            AddInventoryEntities();
        }

        private void RemoveInventoryEntities()
        {
            Manager.GetInstance().RemoveEntities(_allEntities.ToArray());
            _allEntities.Clear();
        }

        private void AddInventoryEntities()
        {
            AddEntitiesToList();
            Manager.GetInstance().AddEntities(GetEntities());
        }

        private void SwapItems(Vector2i firstPoint, Vector2i secondPoint)
        {
            // Switching Items in the All Inventory is unnessary
            if (_activeInventory == _allInventory)
                return;

            Item? firstItem = _activeInventory[firstPoint.Y, firstPoint.X].Item;
            Item? secondItem = _activeInventory[secondPoint.Y, secondPoint.X].Item;

            if ((firstItem == null && secondItem == null) || firstPoint.Equals(secondPoint))
            {
                _firstMovingPoint = null; // Reset the first selected point
                return;
            }

            if (firstItem != null)
            {
                // Remove the old item entity
                _allEntities.Remove(_activeInventory[firstPoint.Y, firstPoint.X].ItemEntity);

                // Remove the old item sprite from the inventory
                Manager.GetInstance().RemoveEntity(_activeInventory[firstPoint.Y, firstPoint.X].ItemEntity);
            }

            if (secondItem != null)
            {
                // Remove the old item entity
                _allEntities.Remove(_activeInventory[secondPoint.Y, secondPoint.X].ItemEntity);

                // Remove the old item sprite from the inventory
                Manager.GetInstance().RemoveEntity(_activeInventory[secondPoint.Y, secondPoint.X].ItemEntity);
            }

            _activeInventory[firstPoint.Y, firstPoint.X].Item = secondItem;
            _activeInventory[secondPoint.Y, secondPoint.X].Item = firstItem;

            if (secondItem != null)
                _allEntities.Add(_activeInventory[firstPoint.Y, firstPoint.X].ItemEntity);

            if (firstItem != null)
                _allEntities.Add(_activeInventory[secondPoint.Y, secondPoint.X].ItemEntity);

            _firstMovingPoint = null; // Reset the first selected point
        }

        private void AddEntitiesToList()
        {
            _allEntities.AddRange(GetAllSlotEntities()); // Add all slot entities
            _allEntities.AddRange(GetTabbarEntities()); // Add the text that will be on the top
            _allEntities.AddRange(GetAllItemEntities()); // Add all Items
            _allEntities.Add(_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection); // Set the selection frame to the starting point
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
                    allSlotEntities.Add(_activeInventory[y, x].SlotBackground);
                }
            }

            return allSlotEntities;
        }

        private List<IEntity> GetAllItemEntities()
        {
            List<IEntity> allItemEntities = new List<IEntity>();

            // Get all inventory slot background entities
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    if (_activeInventory[y, x].HasItem())
                        allItemEntities.Add(_activeInventory[y, x].ItemEntity);
                }
            }

            return allItemEntities;
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
            Sprite firstHorizontalSprite = Asset.GetSprite("GUI.Scroll_Horizontal_Line_1.png", SpriteLayer.ScreenGuiForeground, false);
            firstHorizontalLine.AddComponent(firstHorizontalSprite);
            tabBarEntities.Add(firstHorizontalLine);

            IEntity secondHorizontalLine = new Entity(new Transform(new Vector2(_startPos.X + 6f, horizontalLineY), horizontalLineScale, 0f), "InventoryHorizontalLine");
            Sprite secondHorizontalSprite = Asset.GetSprite("GUI.Scroll_Horizontal_Line_2.png", SpriteLayer.ScreenGuiForeground, false);
            secondHorizontalLine.AddComponent(secondHorizontalSprite);
            tabBarEntities.Add(secondHorizontalLine);

            // Set the first element as active
            tabBarEntities.Add(_tabBar[_playerTabbarPosition.Y, _playerTabbarPosition.X]);

            return tabBarEntities;
        }

        // Calculate the position for every slot
        private void SetInventorySlotPositions(InventorySlot[,] inventorySlots)
        {
            Vector2 position = _startPos;

            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    inventorySlots[y, x] = new InventorySlot(new Transform(position, _slotScalingFactor, 0));

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
            float positionX = _startPos.X - 0.3f;
            float positionY = _startPos.Y + 1.6f;
            float offsetX = 5.8f;
            float offsetY = 1.3f;

            _tabBar[0, 0] = new Entity(new Transform(new Vector2(positionX, positionY), scale, 0f), "Weapons");
            _tabBar[0, 1] = new Entity(new Transform(new Vector2(positionX + offsetX, positionY), scale, 0f), "Materials");
            _tabBar[1, 0] = new Entity(new Transform(new Vector2(positionX, positionY + offsetY), scale, 0f), "All");
            _tabBar[1, 1] = new Entity(new Transform(new Vector2(positionX + offsetX, positionY + offsetY), scale, 0f), "Potions");

            Sprite selectionBackground = Asset.GetSprite("GUI.Scroll_Selection.png", SpriteLayer.ScreenGuiMiddleground, false);

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
            Sprite scrollImage = Asset.GetSprite("GUI.Scroll.png", SpriteLayer.ScreenGuiBackground, false);
            Vector2 middle = new Vector2(scrollImage.Width / 2f, scrollImage.Height / 2f);
            middle *= _scrollScale;

            IEntity background = new Entity(new Transform(new Vector2(0f) - middle, _scrollScale, 0f), "InventoryBackImage");
            background.AddComponent(scrollImage);

            return background;
        }
    }
}