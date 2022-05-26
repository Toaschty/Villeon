using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.ECS;
using Villeon.GUI;
using Villeon.Helper;

namespace Villeon.Components
{
    public class InventorySlot
    {
        private Item? _item;
        private Image _slotBackground;
        private bool _active; // TODO: Find a better name
        private Transform _transform;

        public InventorySlot()
        {
            _item = null;
            _transform = new Transform(Vector2.Zero, 1f, 0f);
            _slotBackground = CreateBackground();
        }

        public InventorySlot(Transform transform)
        {
            _item = null;
            _transform = transform;
            _slotBackground = CreateBackground();
        }

        public Image SlotBackground
        {
            get { return _slotBackground; }
        }

        public Vector2 SlotPosition
        {
            get { return _transform.Position; }

            set 
            {
                _slotBackground.Entity.GetComponent<Transform>().Position = value;
            }
        }

        private Image CreateBackground()
        {
            Image background = new Image("Slot.png", _transform.Position, Vector2.Zero);
            return background;
        }
    }
}
