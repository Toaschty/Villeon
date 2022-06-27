using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.EntityManagement;
using Villeon.GUI;

namespace Villeon.Components
{
    public class Hotbar
    {
        private static Hotbar? _instance;

        private InventorySlot[] _hotbarSlots;
        private InventorySlot[]? _inventoryReferences;

        private List<IEntity> _hotbarEntities;

        private Hotbar()
        {
            _hotbarEntities = new List<IEntity>();
            _hotbarSlots = new InventorySlot[4];
            _inventoryReferences = new InventorySlot[4];

            float offset = 1.2f;
            _hotbarSlots[0] = new InventorySlot(new Transform(new Vector2(4.5f - offset, -5f), 0.3f, 0f));
            _hotbarSlots[1] = new InventorySlot(new Transform(new Vector2(6f - offset, -5f), 0.3f, 0f));
            _hotbarSlots[2] = new InventorySlot(new Transform(new Vector2(7.5f - offset, -5f), 0.3f, 0f));
            _hotbarSlots[3] = new InventorySlot(new Transform(new Vector2(9f - offset, -5f), 0.3f, 0f));
        }

        public static Hotbar GetInstance()
        {
            if (_instance == null)
                _instance = new Hotbar();
            return _instance;
        }

        public void AddItem(int index, InventorySlot slot)
        {
            // Catch wrong input
            if (index > 3 || slot.Item == null)
                return;

            // Add item to hotbar
            _inventoryReferences[index] = slot;

            _hotbarSlots[index].Item = ItemLoader.GetItem(slot.Item.Name);
            _hotbarSlots[index].SetStack(slot.Count);

            // Adjust render layer of all new entities
            foreach (IEntity entity in _hotbarSlots[index].ItemEntites)
            {
                entity.GetComponent<Sprite>().RenderLayer = SpriteLayer.ScreenGuiOverlayForeGround;
            }

            ReloadHotbar();
        }

        public void UseItem(int index)
        {
            if (_inventoryReferences[index] == null)
                return;

            // Decrease item reference by one
            _inventoryReferences[index].DecreaseStack();

            // If stack is now empty -> Remove item from hotbar. Else add reduced item to hotbar again
            if (_inventoryReferences[index].IsStackEmpty())
                RemoveItem(_inventoryReferences[index]);

            UpdateItems();

            // Reload hotbar
            ReloadHotbar();
        }

        public void RemoveItem(InventorySlot slot)
        {
            for (int i = 0; i < _inventoryReferences.Length; i++)
            {
                if (_inventoryReferences[i] == slot)
                {
                    // Remove entities from scene
                    Manager.GetInstance().RemoveEntities(_hotbarSlots[i].ItemEntites);

                    // Reset hotbar slot
                    _hotbarSlots[i] = new InventorySlot(_hotbarSlots[i].Transform);
                    _inventoryReferences[i] = null;
                }
            }
        }

        public void ReloadHotbar()
        {
            // Unload last state
            Manager.GetInstance().RemoveEntities(_hotbarEntities);
            _hotbarEntities.Clear();

            // Load new state
            foreach (InventorySlot slot in _hotbarSlots)
            {
                _hotbarEntities.AddRange(slot.ItemEntites);
            }

            Manager.GetInstance().AddEntities(_hotbarEntities);
        }

        public void UpdateItems()
        {
            for (int i = 0; i < _inventoryReferences.Length; i++)
            {
                if (_inventoryReferences[i] != null)
                {
                    if (_inventoryReferences[i].Item != null)
                        AddItem(i, _inventoryReferences[i]);
                    else
                        RemoveItem(_inventoryReferences[i]);
                }
            }
        }
    }
}
