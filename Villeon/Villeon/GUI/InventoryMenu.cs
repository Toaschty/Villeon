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

namespace Villeon.GUI
{
    public class InventoryMenu : IGUIMenu
    {
        private static InventoryMenu? _inventory;

        private IEntity[] _tabBar = new IEntity[3];

        // Inventory
        private int _inventorySlotsX = 8;
        private int _inventorySlotsY = 5;

        // Align inventory slots
        private Vector2 _startPos = new Vector2(-5.1f, 1.5f);
        private float _slotScalingFactor = 0.3f;
        private float _offset = 0.1f;
        private bool _onSlots = true;
        private float _slotSize;

        // Scroll
        private float _scrollScale = 0.5f;

        private Vector2i _playerInventoryPosition = new Vector2i(0, 0);
        private int _activeTabbar = 0;
        private int _playerTabbarPosition;
        private Vector2i? _swapIndicatorPosition = null;
        private Vector2i? _firstMovingPoint = null;

        // Entities
        private List<IEntity> _inventorySlotEntities = new List<IEntity>(); // Slot background
        private List<IEntity> _tabbarEntities = new List<IEntity>();
        private List<IEntity> _itemEntities = new List<IEntity>(); // Item Entities
        private List<IEntity> _inventorySlotIndicators = new List<IEntity>(); // Slot Selection and Swapping indicators

        // Inventories
        private InventorySlot[,] _activeInventory;
        private InventorySlot[,] _weaponInventory;
        private InventorySlot[,] _potionInventory;
        private InventorySlot[,] _materialInventory;

        private List<IEntity> _itemTextNameEntities = new List<IEntity>();

        private InventoryMenu()
        {
            _slotSize = InventorySlot.SlotSize * _slotScalingFactor;
            _playerTabbarPosition = _activeTabbar;

            _activeInventory = new InventorySlot[_inventorySlotsY, _inventorySlotsX];
            _weaponInventory = new InventorySlot[_inventorySlotsY, _inventorySlotsX];
            _potionInventory = new InventorySlot[_inventorySlotsY, _inventorySlotsX];
            _materialInventory = new InventorySlot[_inventorySlotsY, _inventorySlotsX];

            SetSlotPositions();
            InitializeTabbar();
            AddEntitiesToList();
        }

        public InventorySlot[,] WeaponInventory => _weaponInventory;

        public InventorySlot[,] PotionInventory => _potionInventory;

        public InventorySlot[,] MaterialInventory => _materialInventory;

        public static InventoryMenu GetInstance()
        {
            if (_inventory == null)
                _inventory = new InventoryMenu();
            return _inventory;
        }

        // Get all entities of inventory
        public IEntity[] GetEntities()
        {
            List<IEntity> allEntities = new List<IEntity>();
            allEntities.AddRange(_inventorySlotEntities);
            allEntities.AddRange(_tabbarEntities);
            allEntities.AddRange(_itemEntities);
            allEntities.AddRange(_inventorySlotIndicators);

            return allEntities.ToArray();
        }

        // Add item to inventory. Automatically adds to right type
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
        }

        // Add items to inventory. Automatically adds to right types
        public void AddItems(Item newItem, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                AddItem(newItem);
            }
        }

        public bool OnKeyReleased(Keys key)
        {
            // DEBUG --
            if (key == Keys.G)
                AddItem(ItemLoader.GetItem("HealthPotionSmall"));
            if (key == Keys.B)
                AddItem(ItemLoader.GetItem("PlantShield"));

            if (_onSlots)
                HandleInventorySlot(key);
            else
                HandleTabNavigation(key);

            // Hotbar
            if (_onSlots)
                HandleHotbar(key);

            return true;
        }

        // Return the item which is currently selected -> Null if no item selected
        public Item? GetCurrentlySelectedItem()
        {
            if (_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Item is not null)
                return _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Item;
            return null;
        }

        public InventorySlot GetSlotAtCurrentPosition()
        {
            return _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X];
        }

        // Check if item existing with given amount
        public bool CheckIfExists(string itemName, int amount)
        {
            InventorySlot[,] searchInventory = GetSearchInventory(itemName);

            // Search for item in inventory
            int itemCount = 0;

            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    // Slot has no item -> Add item
                    if (searchInventory[y, x].HasItem() && searchInventory[y, x].Item!.Name == itemName)
                    {
                        itemCount += searchInventory[y, x].Count;
                    }
                }
            }

            if (itemCount >= amount)
                return true;
            return false;
        }

        // Remove amount of items from inventory
        public void RemoveItems(string itemName, int amount)
        {
            InventorySlot[,] searchInventory = GetSearchInventory(itemName);

            // Remove items from non-full stacks
            int removed = RemoveItems(searchInventory, itemName, amount, false);

            // Remove remaining items from full stacks
            RemoveItems(searchInventory, itemName, amount - removed, true);
        }

        // Reload all inventory entities
        public void ReloadItemEntities()
        {
            // Remove all inventory entities
            Manager.GetInstance().RemoveEntities(_itemEntities);
            _itemEntities.Clear();

            // Add all inventory entities
            _itemEntities.AddRange(GetAllItemEntities());
            Manager.GetInstance().RemoveEntities(_itemEntities);
        }

        public void UseItemAtCurrentPosition()
        {
            // Check if current slot has item
            Item? item = GetItemAtCurrentPosition();

            if (item is not null)
            {
                // Get current slot
                InventorySlot slot = GetSlotAtCurrentPosition();

                // If last the item was used -> Reset item; Else decrease item stack
                if (slot.IsStackEmpty())
                {
                    slot.Item = null;
                    RemoveItemNameEntities();
                }
                else
                    slot.DecreaseStack();

                // Reload item entities
                slot.ReloadEntities();

                ReloadItemEntitiesAndRender();
            }
        }

        // Add item to selected inventory. Automatically handles stacking of items
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
                                Manager.GetInstance().RemoveEntities(inventory[y, x].ItemEntites);

                                foreach (IEntity entity in inventory[y, x].ItemEntites)
                                {
                                    _itemEntities.Remove(entity);
                                }

                                inventory[y, x].IncreaseStack();

                                if (inventory == _activeInventory)
                                    _itemEntities.AddRange(inventory[y, x].ItemEntites);

                                // Update hotbar items
                                Hotbar.GetInstance().UpdateItems();

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
                        inventory[y, x].Item = newItem; // Adding new Item

                        if (inventory == _activeInventory)
                            _itemEntities.AddRange(inventory[y, x].ItemEntites);

                        // Update hotbar items
                        Hotbar.GetInstance().UpdateItems();

                        return;
                    }
                }
            }
        }

        // Adds currently selected item to hotbar slot of index
        private void AddItemToHotbar(int index)
        {
            // Hotbar only avaiable for potion inventory
            Item? selected = GetCurrentlySelectedItem();
            if (selected is not null && _activeInventory == _potionInventory)
            {
                Hotbar.GetInstance().AddItem(index, _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X]);
                EquipmentMenu.GetInstance().AddItemInHotbar(index, _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Item!);
            }
        }

        // Handle movement of inventory slotss
        private void HandleInventorySlot(Keys key)
        {
            switch (key)
            {
                // Drop current Item
                case Keys.Q:
                    InventorySlot currentSlot = _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X];

                    if (currentSlot.HasItem())
                    {
                        // Remove Item
                        if (currentSlot.IsStackEmpty())
                        {
                            Manager.GetInstance().RemoveEntities(currentSlot.ItemEntites);
                            currentSlot.Item = null;
                        }
                        else
                        {
                            currentSlot.DecreaseStack();
                        }

                        ReloadItemEntities();

                        Hotbar.GetInstance().UpdateItems();
                    }

                    break;

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
                        // First selected item shouldn't be null
                        if (GetItemAtCurrentPosition() is not null)
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
        }

        // Handle movement of tab slots
        private void HandleTabNavigation(Keys key)
        {
            switch (key)
            {
                // Moving Up
                case Keys.W:
                    break;

                // Moving Down
                case Keys.S:
                    // set the tabbar position to the active tabbar position
                    _playerTabbarPosition = _activeTabbar;
                    _onSlots = true;

                    break;

                // Moving Left
                case Keys.A:
                    if (_playerTabbarPosition > 0)
                        _playerTabbarPosition -= 1;
                    else
                        _playerTabbarPosition = 2;

                    break;

                // Moving Right
                case Keys.D:
                    if (_playerTabbarPosition < 2)
                        _playerTabbarPosition += 1;
                    else
                        _playerTabbarPosition = 0;

                    break;

                case Keys.Space:
                    // Player is changing the tab
                    _activeTabbar = _playerTabbarPosition;
                    _onSlots = true;

                    //Reset swap
                    _firstMovingPoint = null;
                    _swapIndicatorPosition = null;

                    ChangeSelectedInventory(_activeTabbar);
                    break;
            }

            ReloadInventoryIndicators();
        }

        // Change the currently selected inventory
        private void ChangeSelectedInventory(int position)
        {
            IEntity currentTab = _tabBar[position];

            switch (currentTab.Name)
            {
                case "Potions":
                    _activeInventory = _potionInventory;
                    break;
                case "Weapons":
                    _activeInventory = _weaponInventory;
                    break;
                case "Materials":
                    _activeInventory = _materialInventory;
                    break;
            }

            ReloadItemEntitiesAndRender();
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

        // Remove amount of items from inventory -> Option: Remove from full stacks | non-full stacks
        private int RemoveItems(InventorySlot[,] searchInventory, string itemName, int amount, bool stackFullSearch)
        {
            // Search for item in selected inventory
            int removedItems = 0;

            // First remove stacks which are not full
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    // If enough items are removed -> Return
                    if (amount <= removedItems)
                        return removedItems;

                    if ((searchInventory[y, x].HasItem() && searchInventory[y, x].Item!.Name == itemName && stackFullSearch)
                        ||
                        (searchInventory[y, x].HasItem() && searchInventory[y, x].Item!.Name == itemName && !stackFullSearch && !searchInventory[y, x].IsStackFull()))
                    {
                        // Remove all remaining items from slot
                        while (removedItems < amount)
                        {
                            if (searchInventory[y, x].IsStackEmpty())
                            {
                                // Stack is empty -> Remove item
                                searchInventory[y, x].Item = null;
                                removedItems++;

                                // If slot is empty -> Break to next slot
                                break;
                            }
                            else
                            {
                                // Stack is not empty -> Decrease the slot text
                                searchInventory[y, x].DecreaseStack();
                                removedItems++;
                            }

                            searchInventory[y, x].ReloadEntities();
                        }

                        ReloadItemEntities();
                    }
                }
            }

            return removedItems;
        }

        // Return item of current inventory position
        private Item? GetItemAtCurrentPosition()
        {
            return _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].Item;
        }

        // Receive inventory with type of item
        private InventorySlot[,] GetSearchInventory(string itemName)
        {
            Item item = ItemLoader.GetItem(itemName);
            InventorySlot[,] searchInventory = new InventorySlot[_inventorySlotsY, _inventorySlotsX];

            // Depending on type of seach item -> Select correct inventory
            switch (item.ItemType)
            {
                case Item.ITEM_TYPE.MATERIAL:
                    searchInventory = _materialInventory;
                    break;
                case Item.ITEM_TYPE.POTION:
                    searchInventory = _potionInventory;
                    break;
                case Item.ITEM_TYPE.WEAPON:
                    searchInventory = _weaponInventory;
                    break;
            }

            return searchInventory;
        }

        // Reload all inventory entities and add entities to scene
        private void ReloadItemEntitiesAndRender()
        {
            Manager.GetInstance().RemoveEntities(_itemEntities);

            // Add all inventory entities
            _itemEntities.Clear();
            _itemEntities.AddRange(GetAllItemEntities());

            Manager.GetInstance().AddEntities(_itemEntities);
        }

        // Reload inventory indicators (Selections Slots & TabBar)
        private void ReloadInventoryIndicators()
        {
            Manager.GetInstance().RemoveEntities(_inventorySlotIndicators);
            _inventorySlotIndicators.Clear();

            // Set the player cursor if the user is moving inside the Inventory
            if (_onSlots)
                _inventorySlotIndicators.Add(_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);

            // Set the active tabbar indicator when the user is not moving inside the tabbar
            if (_onSlots)
                _inventorySlotIndicators.Add(_tabBar[_activeTabbar]);
            else
                _inventorySlotIndicators.Add(_tabBar[_playerTabbarPosition]);

            // Display the name of the name currently selected
            if (_onSlots)
                _inventorySlotIndicators.AddRange(FillItemText());

            // Display the Swap indicator
            if (_swapIndicatorPosition != null)
                _inventorySlotIndicators.Add(_activeInventory[_swapIndicatorPosition.Value.Y, _swapIndicatorPosition.Value.X].SwapIndicator);

            Manager.GetInstance().AddEntities(_inventorySlotIndicators);
        }

        private List<IEntity> FillItemText()
        {
            _itemTextNameEntities.Clear();

            if (GetCurrentlySelectedItem() is null)
                return _itemTextNameEntities;

            InventorySlot currentSlot = _activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X];
            Vector2 slotPos = currentSlot.Transform.Position;
            Vector2 position = new Vector2(slotPos.X, slotPos.Y);

            // Create text to get the width
            Text itemNameText = new Text(currentSlot.Item!.Name, position, "Alagard", SpriteLayer.ScreenGuiOnTopOfForeground, 0.2f, 1f, 0.2f);
            float textWidth = itemNameText.Width;
            float textHeight = itemNameText.Height;

            // Calculate the new position
            float newPosX = (position.X + (_slotSize / 2)) - (textWidth / 2);
            Vector2 newPos = new Vector2(newPosX, position.Y - textHeight - 0.1f);

            // Create Background
            Sprite popupSprite = Assets.Asset.GetSprite("GUI.Popup.png", SpriteLayer.ScreenGuiForeground, true);
            IEntity textBackground = new Entity(new Transform(new Vector2(newPos.X - 0.13f, newPos.Y - 0.06f), new Vector2((textWidth / popupSprite.Width) + 0.02f, (textHeight / popupSprite.Height) + 0.1f), 0), "ItemTextBackground");
            textBackground.AddComponent(popupSprite);
            _itemTextNameEntities.Add(textBackground);

            itemNameText = new Text(currentSlot.Item!.Name, newPos, "Alagard", SpriteLayer.ScreenGuiOnTopOfForeground, 0.2f, 1f, 0.2f);

            _itemTextNameEntities.AddRange(itemNameText.Letters);

            return _itemTextNameEntities;
        }

        private void RemoveItemNameEntities()
        {
            Manager.GetInstance().RemoveEntities(_itemTextNameEntities);

            foreach (IEntity entity in _itemTextNameEntities)
                _inventorySlotIndicators.Remove(entity);

            _itemTextNameEntities.Clear();
        }

        // Handle the swapping of items
        private void SwapItems(Vector2i firstPoint, Vector2i secondPoint)
        {
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

            // Check if slots are on hotbar
            int[] indicesFirstItemOnHotbar = Hotbar.GetInstance().GetHotbarIndexes(_activeInventory[firstPoint.Y, firstPoint.X]);
            int[] indicesSecondItemOnHotbar = Hotbar.GetInstance().GetHotbarIndexes(_activeInventory[secondPoint.Y, secondPoint.X]);

            if (firstItem != null)
            {
                // Remove the old item entity
                foreach (IEntity entity in _activeInventory[firstPoint.Y, firstPoint.X].ItemEntites)
                {
                    _itemEntities.Remove(entity);
                }

                // Remove the old item sprite from the inventory
                Manager.GetInstance().RemoveEntities(_activeInventory[firstPoint.Y, firstPoint.X].ItemEntites);
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
                _activeInventory[secondPoint.Y, secondPoint.X].Item = null;
            }

            // Swap items
            _activeInventory[firstPoint.Y, firstPoint.X].SetItem(secondItem!, secondPointCount);
            _activeInventory[secondPoint.Y, secondPoint.X].SetItem(firstItem!, firstPointCount);

            // Swap hotbar references for both items
            if (indicesFirstItemOnHotbar.Length > 0)
            {
                for (int i = 0; i < indicesFirstItemOnHotbar.Length; i++)
                    Hotbar.GetInstance().AddItem(indicesFirstItemOnHotbar[i], _activeInventory[secondPoint.Y, secondPoint.X]);
            }

            if (indicesSecondItemOnHotbar.Length > 0)
            {
                for (int i = 0; i < indicesSecondItemOnHotbar.Length; i++)
                    Hotbar.GetInstance().AddItem(indicesSecondItemOnHotbar[i], _activeInventory[firstPoint.Y, firstPoint.X]);
            }

            // Reload all entities of current slots (Add item entity and count sprites)
            _activeInventory[firstPoint.Y, firstPoint.X].ReloadEntities();
            _activeInventory[secondPoint.Y, secondPoint.X].ReloadEntities();

            if (secondItem != null)
                _itemEntities.AddRange(_activeInventory[firstPoint.Y, firstPoint.X].ItemEntites);

            if (firstItem != null)
                _itemEntities.AddRange(_activeInventory[secondPoint.Y, secondPoint.X].ItemEntites);

            _firstMovingPoint = null; // Reset the first selected point

            // Update hotbar
            Hotbar.GetInstance().UpdateItems();
            Hotbar.GetInstance().ReloadHotbar();
        }

        // // Helper functions
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
            float letterScale = 0.35f;

            float positionY = _startPos.Y + 1.6f;

            Text potionText = new Text("Potions", new Vector2(_startPos.X - 1f, positionY), "Alagard", letterSpacing, lineSpacing, letterScale);
            tabBarEntities.AddRange(potionText.GetEntities());

            Text weaponText = new Text("Weapons", new Vector2(_startPos.X + 3f, positionY), "Alagard", letterSpacing, lineSpacing, letterScale);
            tabBarEntities.AddRange(weaponText.GetEntities());

            Text materialText = new Text("Materials", new Vector2(_startPos.X + 7.3f, positionY), "Alagard", letterSpacing, lineSpacing, letterScale);
            tabBarEntities.AddRange(materialText.GetEntities());

            Text legendText = new Text("Use [E] Drop [Q] Swap [Space]", new Vector2(-6f, -4.5f), "Alagard", 0f, 3f, 0.2f);
            tabBarEntities.AddRange(legendText.GetEntities());

            return tabBarEntities;
        }

        // // Init functions
        private void SetSlotPositions()
        {
            SetInventorySlotPositions(_weaponInventory);
            SetInventorySlotPositions(_potionInventory);
            SetInventorySlotPositions(_materialInventory);
            _activeInventory = _potionInventory;
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
                    position += new Vector2(_offset + _slotSize, 0);
                }

                // calculate the position for the new row
                position.X = _startPos.X;
                position.Y = position.Y - _offset - _slotSize;
            }
        }

        private void InitializeTabbar()
        {
            float scale = 0.4f;
            float positionX = _startPos.X - 0.3f;
            float positionY = _startPos.Y + 1.6f;

            _tabBar[0] = new Entity(new Transform(new Vector2(positionX - 1.6f, positionY), scale, 0f), "Potions");
            _tabBar[1] = new Entity(new Transform(new Vector2(positionX + 2.6f, positionY), scale, 0f), "Weapons");
            _tabBar[2] = new Entity(new Transform(new Vector2(positionX + 7f, positionY), scale, 0f), "Materials");

            Sprite selectionBackground = Asset.GetSprite("GUI.Scrolls.Scroll_Selection.png", SpriteLayer.ScreenGuiMiddleground, false);

            for (int i = 0; i < 3; i++)
            {
                _tabBar[i].AddComponent(selectionBackground);
            }
        }

        private void AddEntitiesToList()
        {
            _inventorySlotEntities.AddRange(GetAllSlotEntities());
            _inventorySlotEntities.Add(CreateInventoryBackground());
            _tabbarEntities.AddRange(GetTabbarEntities());
            _itemEntities.AddRange(GetAllItemEntities());

            // Set the starting indicators in the Inventory and tabbar
            _inventorySlotIndicators.Add(_activeInventory[_playerInventoryPosition.Y, _playerInventoryPosition.X].SlotSelection);
            _inventorySlotIndicators.Add(_tabBar[_playerTabbarPosition]);
        }

        private IEntity CreateInventoryBackground()
        {
            Sprite scrollImage = Asset.GetSprite("GUI.Scrolls.Scroll.png", SpriteLayer.ScreenGuiBackground, false);
            Vector2 middle = new Vector2(scrollImage.Width / 2f, scrollImage.Height / 2f);
            middle *= _scrollScale;

            IEntity background = new Entity(new Transform(new Vector2(0f) - middle, _scrollScale, 0f), "InventoryBackImage");
            background.AddComponent(scrollImage);

            return background;
        }
    }
}