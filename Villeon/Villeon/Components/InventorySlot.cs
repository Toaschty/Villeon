using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.ECS;
using Villeon.GUI;
using Villeon.Helper;
using Villeon.Render;

namespace Villeon.Components
{
    public class InventorySlot
    {
        private static float _slotSize = Assets.GetSprite("GUI.Inventory.InventorySlot.png", SpriteLayer.ScreenGuiMiddleground, false).Width;
        private Item? _item;
        private IEntity _slotBackground;
        private IEntity _slotSelection;
        private Transform _transform;

        public InventorySlot(Transform transform)
        {
            _item = null;
            _transform = transform;
            _slotBackground = CreateBackground();
            _slotSelection = CreateSlotSelection();
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

        public Sprite SlotSelectionSprite
        {
            set
            {
                Sprite s = _slotSelection.GetComponent<Sprite>();
                s = value;
            }
        }

        public Vector2 SlotPosition
        {
            get { return _transform.Position; }

            set { _slotBackground.GetComponent<Transform>().Position = value; }
        }

        public IEntity ItemEntity
        {
            get
            {
                IEntity itemEntity = new Entity(_transform, "Item");
                itemEntity.AddComponent(_item!.Sprite);
                return itemEntity;
            }
        }

        public Item Item
        {
            set { _item = value; }
        }

        public bool HasItem()
        {
            if (_item == null)
                return false;
            else
                return true;
        }

        private IEntity CreateBackground()
        {
            IEntity background = new Entity(_transform, "SlotBackground");
            Sprite slotSprite = Assets.GetSprite("GUI.Inventory.InventorySlot.png", SpriteLayer.ScreenGuiMiddleground, false);

            background.AddComponent(slotSprite);

            return background;
        }

        private IEntity CreateSlotSelection()
        {
            float newScale = _transform.Scale.X + 0.02f;
            Vector2 newPos = _transform.Position - new Vector2(0.04f);

            Transform newTransform = new Transform(newPos, newScale, _transform.Degrees);
            IEntity selection = new Entity(newTransform, "SlotSelection");
            Sprite slotSprite = Assets.GetSprite("GUI.Inventory.InventorySlotSelection.png", SpriteLayer.ScreenGuiMiddleground, false);

            selection.AddComponent(slotSprite);

            return selection;
        }
    }
}
