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

        private IEntity[,] _tabBar = new IEntity[2, 2];

        // Inventory
        private int _inventorySlotsX = 8;
        private int _inventorySlotsY = 4;

        // Align inventory slots
        private Vector2 _startPos = new Vector2(-5.1f, 0.3f);
        private float _slotScalingFactor = 0.3f;
        private float _slotSize = InventorySlot.SlotSize;
        private float _offset = 0.1f;
        private bool _onSlots = true;

        // Scroll
        private float _scrollScale = 0.5f;

        private Vector2i _playerInventoryPosition = new Vector2i(0, 0);
        private Vector2i _playerTabbarPosition = new Vector2i(0, 1);
        private Vector2i _activeTabbar = new Vector2i(0, 1);
        private Vector2i? _swapIndicatorPosition = null;
        private Vector2i? _firstMovingPoint = null;

        // Entities
        private List<IEntity> _inventorySlotEntities = new List<IEntity>(); // Slot background
        private List<IEntity> _tabbarEntities = new List<IEntity>();
        private List<IEntity> _itemEntities = new List<IEntity>(); // Item Entities
        private List<IEntity> _inventorySlotIndicators = new List<IEntity>(); // Slot Selection and Swapping indicators

        // Inventories
        private InventorySlot[,] _activeInventory;
        private InventorySlot[,] _allInventory;
        private InventorySlot[,] _weaponInventory;
        private InventorySlot[,] _potionInventory;
        private InventorySlot[,] _materialInventory;

        // VillageOverlay
        private VillageOverlay? _villageOverlay;

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

        public void SetVillageOverlay(VillageOverlay overlay)
        {
            _villageOverlay = overlay;
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
            List<IEntity> allEntities = new List<IEntity>();
            allEntities.AddRange(_inventorySlotEntities);
            allEntities.AddRange(_tabbarEntities);
            allEntities.AddRange(_itemEntities);
            allEntities.AddRange(_inventorySlotIndicators);

            return allEntities.ToArray();
        }

        public bool OnKeyReleased(Keys key)
        {
            if (key == Keys.H)
            {
                AddItem(ItemLoader.GetItem("Sword"));
                //Console.WriteLine("Spawning Sword!");
            }
            else if (key == Keys.G)
            {
                AddItem(ItemLoader.GetItem("HealthPotion"));
                //Console.WriteLine("Spawning Health potion!");
            }

            if (_onSlots)
                HandleInventorySlot(key);
            else
                HandleTabNavigation(key);

            // Hotbar
            if (_onSlots)
                HandleHotbar(key);

            return true;
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

        public void UseItemAtCurrentPosition()
        {
            Item? item = GetActiveItem();
            IEntity[] itemEntity = GetActiveItemEntity();

            if (item is not null)
            {
                InventorySlot currentSlot = _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X];

                if (currentSlot.IsStackEmpty())
                {
                    // Stack is empty -> Remove item
                    currentSlot.Item = null;
                }
                else
                {
                    // Stack is not empty -> Decrease the slot text
                    currentSlot.DecreaseStack();
                }

                ReloadItemEntities();
                if (_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].HasItem())
                    Console.WriteLine("Item: " + _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Item!.Name + " ItemCount: " + _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Count);
                else
                    Console.WriteLine("Item: Null " + " ItemCount: " + _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Count);
            }
        }

        private void HandleHotbar(Keys key)
        {
            switch (key)
            {
                case Keys.D1: AddItemToHotbar(0); break;
                case Keys.D2: AddItemToHotbar(1); break;
                case Keys.D3: AddItemToHotbar(2); break;
                case Keys.D4: AddItemToHotbar(3); break;
            }
        }

        private void AddItemToHotbar(int index)
        {
            if (GetCurrentlySelectedItem() is not null && _activeInventory == _potionInventory)
            {
                _villageOverlay!.AddItem(index, _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X], GetCurrentlySelectedItem() !);
            }
        }

        private Item? GetActiveItem()
        {
            return _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Item;
        }

        private IEntity[] GetActiveItemEntity()
        {
            return _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].ItemEntites;
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
                    Manager.GetInstance().RemoveEntities(_allInventory[y, x].ItemEntites);

                    foreach (IEntity entity in _allInventory[y, x].ItemEntites)
                    {
                        _itemEntities.Remove(entity);
                    }

                    _allInventory[y, x] = new InventorySlot(_allInventory[y, x].Transform);
                }
            }

            // Get all weapons
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    for (int i = 0; i < _weaponInventory[y, x].Count; i++)
                    {
                        allItems.Add(_weaponInventory[y, x].Item!);
                    }
                }
            }

            // Get all potions
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    if (_potionInventory[y, x].HasItem())
                    {
                        for (int i = 0; i < _potionInventory[y, x].Count; i++)
                        {
                            allItems.Add(_potionInventory[y, x].Item!);
                        }
                    }
                }
            }

            // Get all materials
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    if (_materialInventory[y, x].HasItem())
                    {
                        for (int i = 0; i < _materialInventory[y, x].Count; i++)
                        {
                            allItems.Add(_materialInventory[y, x].Item!);
                        }
                    }
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
                    // Slot has already a Item -> Stack Item
                    if (inventory[y, x].HasItem())
                    {
                        // Check if the slot has the same item
                        if (inventory[y, x].Item!.Name == newItem.Name)
                        {
                            // Check if the stack is not already full
                            if (!inventory[y, x].IsStackFull())
                            {
                                // Remove all Item entities
                                foreach (IEntity entity in inventory[y, x].ItemEntites)
                                {
                                    _itemEntities.Remove(entity);
                                }

                                inventory[y, x].IncreaseStack();

                                if (inventory == _activeInventory)
                                    _itemEntities.AddRange(inventory[y, x].ItemEntites);

                                return;
                            }
                        }
                    }
                }
            }

            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    // Slot has no item -> Add item
                    if (!inventory[y, x].HasItem())
                    {
                        //Console.WriteLine("NEW ITEM");
                        inventory[y, x].Item = newItem; // Adding new Item

                        if (inventory == _activeInventory)
                            _itemEntities.AddRange(inventory[y, x].ItemEntites);

                        return;
                    }
                }
            }
        }

        private void HandleInventorySlot(Keys key)
        {
            switch (key)
            {
                // Moving Up
                case Keys.W:
                    if (_playerInventoryPosition.Y > 0)
                        _playerInventoryPosition.Y -= 1;
                    else
                        _onSlots = false;

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
                    if (_firstMovingPoint is null)
                    {
                        // Swapping is not allowed in the all inventory
                        // First selected item shouldn't be null
                        if (GetActiveItem() is not null && _activeInventory != _allInventory)
                        {
                            // First point not set
                            _firstMovingPoint = new Vector2i(_playerInventoryPosition.X, _playerInventoryPosition.Y);

                            _swapIndicatorPosition = new Vector2i(_playerInventoryPosition.X, _playerInventoryPosition.Y);
                        }
                    }
                    else
                    {
                        Vector2i secondPoint = new Vector2i(_playerInventoryPosition.X, _playerInventoryPosition.Y);
                        SwapItems((Vector2i)_firstMovingPoint, secondPoint);
                        _swapIndicatorPosition = null;
                    }

                    break;
            }

            ReloadInventoryIndicators();

            //if (_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].HasItem())
            //    Console.WriteLine("Item: " + _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Item!.Name + " ItemCount: " + _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Count);
            //else
            //    Console.WriteLine("Item: Null " + " ItemCount: " + _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Count);
        }

        private void HandleTabNavigation(Keys key)
        {
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
                        //Console.WriteLine("ActiveTabbar: " + _activeTabbar + " || Player: " + _playerTabbarPosition);
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
                    _onSlots = true;

                    //Reset swap
                    _firstMovingPoint = null;
                    _swapIndicatorPosition = null;

                    ChangeInventory(_activeTabbar);
                    ReloadInventoryIndicators();
                    break;
            }

            ReloadInventoryIndicators();
        }

        private void ChangeInventory(Vector2i tabbarPosition)
        {
            IEntity currentTab = _tabBar[tabbarPosition.Y, tabbarPosition.X];

            switch (currentTab.Name)
            {
                case "All":
                    //Console.WriteLine("All");
                    SetupAllInventory();
                    _activeInventory = _allInventory;
                    break;
                case "Materials":
                    //Console.WriteLine("Materials");
                    _activeInventory = _materialInventory;
                    break;
                case "Potions":
                    //Console.WriteLine("Potions");
                    _activeInventory = _potionInventory;
                    break;
                case "Weapons":
                    //Console.WriteLine("Weapons");
                    _activeInventory = _weaponInventory;
                    break;
            }

            ReloadItemEntities();
        }

        private void ReloadItemEntities()
        {
            //Console.WriteLine("RELOADING ITEMS!");
            Manager.GetInstance().RemoveEntities(_itemEntities);
            _itemEntities.Clear();
            _itemEntities.AddRange(GetAllItemEntities());
            Manager.GetInstance().AddEntities(_itemEntities);
        }

        private void ReloadInventoryIndicators()
        {
            Manager.GetInstance().RemoveEntities(_inventorySlotIndicators);
            _inventorySlotIndicators.Clear();

            // Set the player cursor if the user is moving inside the Inventory
            if (_onSlots)
                _inventorySlotIndicators.Add(_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);

            // Set the active tabbar indicator when the user is not moving inside the tabbar
            if (_onSlots)
                _inventorySlotIndicators.Add(_tabBar[_activeTabbar.Y, _activeTabbar.X]);

            _inventorySlotIndicators.Add(_tabBar[_playerTabbarPosition.Y, _playerTabbarPosition.X]);

            if (_swapIndicatorPosition != null)
                _inventorySlotIndicators.Add(_activeInventory[_swapIndicatorPosition.Value.Y, _swapIndicatorPosition.Value.X].SwapIndicator);

            Manager.GetInstance().AddEntities(_inventorySlotIndicators);
        }

        private void SwapItems(Vector2i firstPoint, Vector2i secondPoint)
        {
            // Switching Items in the All Inventory is unnessary
            if (_activeInventory == _allInventory)
                return;

            Item? firstItem = _activeInventory[firstPoint.Y, firstPoint.X].Item;
            Item? secondItem = _activeInventory[secondPoint.Y, secondPoint.X].Item;

            if (firstItem == null || firstPoint.Equals(secondPoint))
            {
                _firstMovingPoint = null; // Reset the first selected point
                return;
            }

            // Save count of items
            int firstPointCount = _activeInventory[firstPoint.Y, firstPoint.X].Count;
            int secondPointCount = _activeInventory[secondPoint.Y, secondPoint.X].Count;

            if (firstItem != null)
            {
                // Remove the old item entity
                foreach (IEntity entity in _activeInventory[firstPoint.Y, firstPoint.X].ItemEntites)
                {
                    _itemEntities.Remove(entity);
                }

                // Remove the old item sprite from the inventory
                Manager.GetInstance().RemoveEntities(_activeInventory[firstPoint.Y, firstPoint.X].ItemEntites);
                _activeInventory[firstPoint.Y, firstPoint.X].UnloadCountText();
                _activeInventory[firstPoint.Y, firstPoint.X].Item = null;
            }

            if (secondItem != null)
            {
                // Remove the old item entity
                foreach (IEntity entity in _activeInventory[secondPoint.Y, secondPoint.X].ItemEntites)
                {
                    _itemEntities.Remove(entity);
                }

                // Remove the old item sprite from the inventory
                Manager.GetInstance().RemoveEntities(_activeInventory[secondPoint.Y, secondPoint.X].ItemEntites);
                _activeInventory[secondPoint.Y, secondPoint.X].UnloadCountText();
                _activeInventory[secondPoint.Y, secondPoint.X].Item = null;
            }

            // Swap items
            _activeInventory[firstPoint.Y, firstPoint.X].SetItem(secondItem!, secondPointCount);
            _activeInventory[secondPoint.Y, secondPoint.X].SetItem(firstItem!, firstPointCount);

            // Load Counter Text
            _activeInventory[firstPoint.Y, firstPoint.X].LoadCountText();
            _activeInventory[secondPoint.Y, secondPoint.X].LoadCountText();

            if (secondItem != null)
                _itemEntities.AddRange(_activeInventory[firstPoint.Y, firstPoint.X].ItemEntites);

            if (firstItem != null)
                _itemEntities.AddRange(_activeInventory[secondPoint.Y, secondPoint.X].ItemEntites);

            _firstMovingPoint = null; // Reset the first selected point
        }

        private void AddEntitiesToList()
        {
            _inventorySlotEntities.AddRange(GetAllSlotEntities());
            _inventorySlotEntities.Add(CreateInventoryBackground());
            _tabbarEntities.AddRange(GetTabbarEntities());
            _itemEntities.AddRange(GetAllItemEntities());

            // Set the starting indicators in the Inventory and tabbar
            _inventorySlotIndicators.Add(_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);
            _inventorySlotIndicators.Add(_tabBar[_playerTabbarPosition.Y, _playerTabbarPosition.X]);
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
                        allItemEntities.AddRange(_activeInventory[y, x].ItemEntites);
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

            Text weaponText = new Text("Weapons", new Vector2(_startPos.X + 0.1f, positionY - 0.4f), "Alagard", letterSpacing, lineSpacing, letterScale);
            tabBarEntities.AddRange(weaponText.GetEntities());

            Text potionText = new Text("Potions", new Vector2(_startPos.X + 6.2f, positionY + 0.8f), "Alagard", letterSpacing, lineSpacing, letterScale);
            tabBarEntities.AddRange(potionText.GetEntities());

            Text materialText = new Text("Materials", new Vector2(_startPos.X + 5.7f, positionY - 0.4f), "Alagard", letterSpacing, lineSpacing, letterScale);
            tabBarEntities.AddRange(materialText.GetEntities());

            Text legendText = new Text("Use [E] Drop [Q] Swap [Space]", new Vector2(-6f, -4.5f), "Alagard", 0f, 3f, 0.2f);
            tabBarEntities.AddRange(legendText.GetEntities());

            float horizontalLineY = _startPos.Y + 2.5f;
            float horizontalLineScale = 0.3f;

            // Make HorizontalLines
            IEntity firstHorizontalLine = new Entity(new Transform(new Vector2(_startPos.X + 0.2f, horizontalLineY), horizontalLineScale, 0f), "InventoryHorizontalLine");
            Sprite firstHorizontalSprite = Asset.GetSprite("GUI.Scroll_Horizontal_Line_1.png", SpriteLayer.ScreenGuiForeground, false);
            firstHorizontalLine.AddComponent(firstHorizontalSprite);
            tabBarEntities.Add(firstHorizontalLine);

            IEntity secondHorizontalLine = new Entity(new Transform(new Vector2(_startPos.X + 6f, horizontalLineY), horizontalLineScale, 0f), "InventoryHorizontalLine");
            Sprite secondHorizontalSprite = Asset.GetSprite("GUI.Scroll_Horizontal_Line_2.png", SpriteLayer.ScreenGuiForeground, false);
            secondHorizontalLine.AddComponent(secondHorizontalSprite);
            tabBarEntities.Add(secondHorizontalLine);

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