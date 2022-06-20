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

            _itemEntites.Add(_itemEntity);
        }

        public static float SlotSize
        {
            get => _slotSize;
        }

        public IEntity SlotBackground
        {
            get { return _slotBackground; }
        }

        public IEntity SlotSelection
        {
            get { return _slotSelection; }
        }

        public IEntity SwapIndicator
        {
            get { return _slotSwapIndicator; }
        }

        public IEntity ItemEntity
        {
            get
            {
                return _itemEntity;
            }
        }

        public Transform Transform => _transform;

        public IEntity[] ItemEntites
        {
            get { return _itemEntites.ToArray(); }
            set { _itemEntites = new List<IEntity>(); }
        }

        public int Count => _itemCount;

        public IEntity[] TextEntities => _itemCountText != null ? _itemCountText.GetEntities() : new IEntity[0];

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
                    _itemStackSize = 0;
                    _itemCount = 0;
                }
                else
                {
                    _itemEntity.AddComponent(_item!.Sprite);
                    _itemStackSize = _item!.StackSize;
                    _itemCount = 1;
                }
            }
        }

        public void SetItem(Item item, int itemCount)
        {
            Item = item;
            _itemCount = itemCount;
        }

        public bool HasItem()
        {
            if (_item == null)
                return false;
            else
                return true;
        }

        public bool IsStackFull()
        {
            if (_itemStackSize == 0)
                return false;

            if (_itemCount < _itemStackSize)
                return false;
            else
                return true;
        }

        public void SetStack(int count)
        {
            if (count > _itemStackSize)
                _itemCount = _itemStackSize;
            else
                _itemCount = count;
        }

        public void IncreaseStack()
        {
            if (_itemCount < _itemStackSize)
                _itemCount++;

            if (_itemCountText != null)
            {
                Manager.GetInstance().RemoveEntities(_itemCountText.GetEntities());
                foreach (IEntity entity in _itemCountText.GetEntities())
                {
                    _itemEntites.Remove(entity);
                }
            }

            // Refresh the text
            _itemCountText = new Text(_itemCount.ToString(), _transform.Position, "Alagard", SpriteLayer.ScreenGuiOnTopOfForeground, 0.1f, 1f, 0.2f);
            _itemEntites.AddRange(_itemCountText.GetEntities());
        }

        public void UnloadCountText()
        {
            if (_itemCountText == null)
                return;

            Manager.GetInstance().RemoveEntities(_itemCountText!.GetEntities());
            foreach (IEntity entity in _itemCountText!.GetEntities())
            {
                _itemEntites?.Remove(entity);
            }

            _itemCountText = null;
        }

        public void LoadCountText()
        {
            if (_item == null || _itemCount == 1)
                return;

            _itemCountText = new Text(_itemCount.ToString(), _transform.Position, "Alagard", SpriteLayer.ScreenGuiOnTopOfForeground, 0.1f, 1f, 0.2f);
            Manager.GetInstance().AddEntities(_itemCountText!.GetEntities());
            _itemEntites.AddRange(_itemCountText.GetEntities());
        }

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
