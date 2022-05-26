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
    public class Inventory
    {
        private IEntity _backgroundImage;
        private InventorySlot[,] _inventorySlots;
        private int _inventorySlotsX = 5;
        private int _inventorySlotsY = 5;

        private Vector2 _startPos = new Vector2(-0.75f, -0.6f);
        private float _slotSize = 0.5f;
        private float _offsetX = 0.4f;
        private float _offsetY = 0.4f;

        public Inventory()
        {
            _backgroundImage = CreateFrame();
            _inventorySlots = new InventorySlot[_inventorySlotsY, _inventorySlotsX];

            FillInventorySlots();
        }

        public void Enable()
        {
            DisplayInventory();
        }

        public void Disable()
        {
            RemoveInventory();
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

        private void DisplayInventory()
        {
            Console.WriteLine("DISPLAY!!!");
            Manager manager = Manager.GetInstance();

            /*
            for (int y = 0; y < _inventorySlotsY; y++)
            {
                for (int x = 0; x < _inventorySlotsX; x++)
                {
                    manager.AddEntity(_inventorySlots[y, x].SlotBackground.Entity);
                    Console.WriteLine("X: " + x + " Y: " + y + " Pos: " + _inventorySlots[y, x].SlotBackground.Entity.GetComponent<Transform>().Position);
                }
            }
            */

            _inventorySlots[0, 0].SlotPosition = new Vector2(0.5f, 0.5f);
            manager.AddEntity(_inventorySlots[0, 0].SlotBackground.Entity);
            Console.WriteLine(_inventorySlots[0, 0].SlotBackground.Entity.GetComponent<Transform>().Position);

            //manager.AddEntity(_backgroundImage);
        }

        private void RemoveInventory()
        {
            Manager manager = Manager.GetInstance();

            for (int i = 0; i < _inventorySlotsY; i++)
            {
                for (int j = 0; j < _inventorySlotsX; j++)
                {
                    manager.RemoveEntity(_inventorySlots[i, j].SlotBackground.Entity);
                }
            }

            manager.RemoveEntity(_backgroundImage);
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
