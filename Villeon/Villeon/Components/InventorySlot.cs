﻿using System;
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
        public static float SlotSize = Assets.GetSprite("GUI.Slot.png", SpriteLayer.ScreenGuiMiddleground, false).Width;
        private Item? _item;
        private IEntity _slotBackground;
        private bool _active; //TODO: Find a better name
        private Transform _transform;

        public InventorySlot(Transform transform)
        {
            _item = null;
            _transform = transform;
            _slotBackground = CreateBackground();
        }

        public IEntity SlotBackground
        {
            get { return _slotBackground; }
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
                itemEntity.AddComponent(_item.Sprite);
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
            Sprite slotSprite = Assets.GetSprite("GUI.Slot.png", SpriteLayer.ScreenGuiMiddleground, true);
            background.AddComponent(slotSprite);

            return background;
        }
    }
}
