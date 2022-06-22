using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.GUI
{
    public class InventorySlot
    {
        private static float _slotSize = Asset.GetSprite("GUI.Inventory.InventorySlot.png", SpriteLayer.ScreenGuiMiddleground, false).Width;
        private Item? _item;
        private int _itemCount;
        private int _itemStackSize;
        private IEntity _slotBackground;
        private IEntity _slotSelection;
        private IEntity _slotSwapIndicator;
        private IEntity _itemEntity;
        private Text? _itemCountText;
        private Transform _transform;

        private List<IEntity> _itemEntites;

        public InventorySlot(Transform transform)
        {
            _itemEntites = new List<IEntity>();
            _item = null;
            _transform = transform;
            _slotBackground = CreateBackground();
            _slotSelection = CreateSlotSelection();
            _slotSwapIndicator = CreateSwapIndicator();
            _itemEntity = new Entity(transform, "ItemEntity");
            _itemCountText = null;
            _itemCount = 0;
            _itemStackSize = 0;

            _itemEntites.Add(_itemEntity);
        }

        public static float SlotSize => _slotSize;

        public IEntity SlotBackground => _slotBackground;

        public IEntity SlotSelection => _slotSelection;

        public IEntity SwapIndicator => _slotSwapIndicator;

        public Transform Transform => _transform;

        public List<IEntity> ItemEntites
        {
            get { return _itemEntites; }
            set { _itemEntites = new List<IEntity>(); }
        }

        public int Count => _itemCount;

        public Item? Item
        {
            get
            {
                return _item;
            }

            set
            {
                _item = value;

                if (value == null)
                {
                    // Reset all item data from slot
                    _itemStackSize = 0;
                    _itemCount = 0;
                    _itemEntites.Clear();
                }
                else
                {
                    // Add item sprite
                    _itemEntity.RemoveComponent<Sprite>();
                    _itemEntity.AddComponent(_item!.Sprite);

                    // Set item specific data
                    _itemStackSize = _item!.StackSize;
                    _itemCount = 1;
                }
            }
        }

        // Set item with count
        public void SetItem(Item item, int itemCount)
        {
            Item = item;
            _itemCount = itemCount;
        }

        // Check if slot has item
        public bool HasItem()
        {
            if (_item == null)
                return false;
            else
                return true;
        }

        // Check if stack limit is reached
        public bool IsStackFull()
        {
            if (_itemCount < _itemStackSize)
                return false;
            else
                return true;
        }

        // Check if stack is empty
        public bool IsStackEmpty()
        {
            if (_itemCount <= 1)
                return true;
            else
                return false;
        }

        // Change stack count of current item
        public void SetStack(int count)
        {
            if (count > _itemStackSize)
                _itemCount = _itemStackSize;
            else
                _itemCount = count;

            ReloadEntities();
        }

        // Increase stack by one
        public void IncreaseStack()
        {
            if (_itemCount < _itemStackSize)
                _itemCount++;

            // Remove old count text
            if (_itemCountText != null)
                Manager.GetInstance().RemoveEntities(_itemCountText.GetEntities());

            // Reload all item entities
            ReloadEntities();
        }

        // Reduce stack by one
        public void DecreaseStack()
        {
            if (_itemCount > 1)
                _itemCount--;

            // Remove old count text
            if (_itemCountText != null)
                Manager.GetInstance().RemoveEntities(_itemCountText.GetEntities());

            ReloadEntities();
        }

        // Reload all slot entities -> Spawn new text
        public void ReloadEntities()
        {
            _itemEntites.Clear();

            _itemCountText = new Text(_itemCount.ToString(), _transform.Position, "Alagard", SpriteLayer.ScreenGuiOnTopOfForeground, 0.1f, 1f, 0.2f);
            _itemEntites.AddRange(_itemCountText.GetEntities());
            _itemEntites.Add(_itemEntity);
        }

        // Init functions
        private IEntity CreateBackground()
        {
            IEntity background = new Entity(_transform, "SlotBackground");
            Sprite slotSprite = Asset.GetSprite("GUI.Inventory.InventorySlot.png", SpriteLayer.ScreenGuiMiddleground, false);

            background.AddComponent(slotSprite);

            return background;
        }

        private IEntity CreateSlotSelection()
        {
            float newScale = _transform.Scale.X + 0.02f;
            Vector2 newPos = _transform.Position - new Vector2(0.04f);

            Transform newTransform = new Transform(newPos, newScale, _transform.Degrees);
            IEntity selection = new Entity(newTransform, "SlotSelection");
            Sprite slotSprite = Asset.GetSprite("GUI.Inventory.InventorySlotSelection.png", SpriteLayer.ScreenGuiMiddleground, false);

            selection.AddComponent(slotSprite);

            return selection;
        }

        private IEntity CreateSwapIndicator()
        {
            IEntity swapIndicator = new Entity(_transform, "Swap Indicator");
            swapIndicator.AddComponent(Asset.GetSprite("GUI.Inventory.InventorySlotSwapIndicator.png", SpriteLayer.ScreenGuiForeground, true));
            AnimationController controller = new AnimationController();
            controller.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.InventorySlotSwapIndicator.png", 0.1f));
            swapIndicator.AddComponent(controller);

            return swapIndicator;
        }
    }
}
