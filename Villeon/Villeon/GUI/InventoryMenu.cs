using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;
using Villeon.Render;

namespace Villeon.GUI
{
    public class InventoryMenu : IGUIMenu
    {
        private IEntity _backgroundImage;
        private InventorySlot[,] _inventorySlots;

        // Inventory
        private int _inventorySlotsX = 7;
        private int _inventorySlotsY = 4;

        // Align inventory slots
        private Vector2 _startPos = new Vector2(-6f, 1f);
        private float _slotScalingFactor = 0.4f;
        private float _slotSize = InventorySlot.SlotSize;
        private float _offset = 0.2f;

        // Scroll
        private float _scrollScale = 0.5f;

        public InventoryMenu()
        {
            _backgroundImage = CreateInventoryBackground();
            _inventorySlots = new InventorySlot[_inventorySlotsY, _inventorySlotsX];

            FillInventorySlots();
        }

        public IEntity[] GetEntities()
        {
            IEntity[] entities = new Entity[_inventorySlots.Length + 1];

            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    entities[(_inventorySlotsX * y) + x] = _inventorySlots[y, x].SlotBackground;
                }
            }

            entities[_inventorySlots.Length] = _backgroundImage;
            return entities;
        }

        // Calculate the position for every slot
        private void FillInventorySlots()
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
