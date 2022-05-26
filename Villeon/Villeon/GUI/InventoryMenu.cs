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

        private Vector2 _startPos = new Vector2(-0.75f, -0.6f);
        private float _slotSize = 0.5f;
        private float _offsetX = 0.4f;
        private float _offsetY = 0.4f;

        public InventoryMenu()
        {
            _backgroundImage = CreateFrame();
            _inventorySlots = new InventorySlot[_inventorySlotsY, _inventorySlotsX];

            FillInventorySlots();
        }

        public IEntity[] GetEntities()
        {
            IEntity[] entities = new IEntity[_inventorySlots.Length];

            for (int i = 0; i < _inventorySlotsY; i++)
            {
                for (int j = 0; j < _inventorySlotsX; j++)
                {
                    entities[i] = _inventorySlots[i, j].SlotBackground.Entity;
                }
            }

            return entities;
        }

        // Calculate the position for every slot
        private void FillInventorySlots()
        {
            Vector2 previousSlotPos;
            Vector2 pos;
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    // TODO: Set to start pos
                    if (y == 0 && x == 0)
                    {
                        _inventorySlots[y, x] = new InventorySlot(new Transform(new Vector2(-1f, 0), 1f, 0));
                        continue;
                    }

                    if (y == 0 && x != 0)
                        previousSlotPos = _inventorySlots[y, x - 1].SlotPosition;
                    else if (x == 0 && y != 0)
                        previousSlotPos = _inventorySlots[y - 1, x].SlotPosition;
                    else
                        previousSlotPos = _inventorySlots[y - 1, x - 1].SlotPosition;

                    pos = new Vector2(previousSlotPos.X + _offsetX, previousSlotPos.Y + _offsetY);

                    Console.WriteLine("X: " + x + " Y: " + y + " Pos: " + pos);

                    _inventorySlots[y, x] = new InventorySlot(new Transform(pos, 1f, 0));
                }
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
