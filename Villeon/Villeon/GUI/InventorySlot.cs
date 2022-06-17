using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;
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
        private IEntity _itemEntity;
        private Text? _itemCountText;
        private Transform _transform;

        public InventorySlot(Transform transform)
        {
            _item = null;
            _transform = transform;
            _slotBackground = CreateBackground();
            _slotSelection = CreateSlotSelection();
            _itemEntity = new Entity(transform, "ItemEntity");
            _itemCountText = null;
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

        public IEntity ItemEntity
        {
            get
            {
                _itemEntity.AddComponent(_item!.Sprite);
                return _itemEntity;
            }
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
                    _itemStackSize = 0;
                    _itemCount = 0;
                }
                else
                {
                    _itemStackSize = _item!.StackSize;
                    _itemCount = 1;
                }
            }
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

        public void IncreaseStack()
        {
            if (_itemCountText != null)
            {
                Console.WriteLine("Writing Text! " + _itemCountText.GetEntities().Length);
                Manager.GetInstance().RemoveEntities(_itemCountText.GetEntities());
            }

            Console.WriteLine("Increasing Stack: " + _itemCount + " | StackSize: " + _itemStackSize);
            _itemCountText = new Text(_itemCount.ToString(), _transform.Position, "Alagard");
            Manager.GetInstance().AddEntities(_itemCountText.GetEntities());

            if (_itemCount < _itemStackSize)
                _itemCount++;
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
    }
}
