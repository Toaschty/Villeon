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
        private int _inventorySlotsX = 5;
        private int _inventorySlotsY = 5;

        private Vector2 _startPos = new Vector2(-0.49f, 0.4f);
        private float _scale = 0.0285f;
        private float _slotSize = 0.114f;
        private float _offset = 0.02f;

        public InventoryMenu()
        {
            _backgroundImage = CreateFrame();
            _inventorySlots = new InventorySlot[_inventorySlotsY, _inventorySlotsX];

            InitializeSlots();
            FillInventorySlots();
        }

        public IEntity[] GetEntities()
        {
            IEntity[] entities = new Entity[_inventorySlots.Length + 1];

            for (int i = 0; i < _inventorySlotsY; i++)
            {
                for (int j = 0; j < _inventorySlotsX; j++)
                {
                    entities[(_inventorySlotsX * i) + j] = _inventorySlots[i, j].SlotBackground;
                }
            }

            entities[_inventorySlots.Length] = _backgroundImage;

            return entities;
        }

        private void InitializeSlots()
        {
            for (int i = 0; i < _inventorySlotsY; i++)
            {
                for (int j = 0; j < _inventorySlotsX; j++)
                {
                    _inventorySlots[i, j] = new InventorySlot(new Transform(_startPos, _scale, 0));
                }
            }
        }

        // Calculate the position for every slot
        private void FillInventorySlots()
        {
            Vector2 position = _startPos;
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    _inventorySlots[y, x] = new InventorySlot(new Transform(position, _scale, 0));

                    // calc new pos
                    position += new Vector2(_offset + _slotSize, 0);
                }

                position.X = _startPos.X;
                position.Y = position.Y - _offset - _slotSize;
            }
        }

        private IEntity CreateFrame()
        {
            IEntity background = new Entity(new Transform(new Vector2(-0.75f, -0.6f), 0.04f, 0f), "InventoryBackImage");
            //Sprite scrollImage = Assets.GetSprite("GUI.Scroll.png", SpriteLayer.ScreenGuiBackground, false);
            Sprite scrollImage = Assets.GetSprite("GUI.Scroll.png", SpriteLayer.ScreenGuiBackground, false);
            background.AddComponent(scrollImage);

            return background;
        }
    }
}
