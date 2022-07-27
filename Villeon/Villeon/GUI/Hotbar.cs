using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.GUI
{
    public class Hotbar
    {
        private static Hotbar? _instance;

        private InventorySlot[] _hotbarSlots;
        private InventorySlot?[]? _inventoryReferences;

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
            _inventoryReferences![index] = slot;

            _hotbarSlots[index].Item = ItemLoader.GetItem(slot.Item.Name);
            _hotbarSlots[index].SetStack(slot.Count);

            // Adjust render layer of all new entities
            foreach (IEntity entity in _hotbarSlots[index].ItemEntites)
            {
                entity.GetComponent<Sprite>().RenderLayer = SpriteLayer.ScreenGuiOverlayForeGround;
            }

            ReloadHotbar();
        }

        public void UseItem(int index, Health playerHealth)
        {
            if (_inventoryReferences![index] == null)
                return;

            // If stack is empty -> Remove item from hotbar. Else decrease item reference by one
            if (_inventoryReferences[index]!.IsStackEmpty())
                RemoveItem(_inventoryReferences[index]!);
            else
                _inventoryReferences[index]!.DecreaseStack();

            UpdateItems();

            // Apply effect
            playerHealth.Heal(20);

            // Reload hotbar
            ReloadHotbar();
        }

        public void RemoveItem(InventorySlot slot)
        {
            for (int i = 0; i < _inventoryReferences!.Length; i++)
            {
                if (_inventoryReferences[i] == slot)
                {
                    // Remove entities from scene
                    Manager.GetInstance().RemoveEntities(_hotbarSlots[i].ItemEntites);

                    // Remove item from inventory
                    _inventoryReferences[i]!.Item = null;

                    // Reset hotbar slot
                    _hotbarSlots[i] = new InventorySlot(_hotbarSlots[i].Transform);
                    _inventoryReferences[i] = null;

                    // Remove item from equipment view
                    EquipmentMenu.GetInstance().RemoveItemInHotbar(i);
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
            for (int i = 0; i < _inventoryReferences!.Length; i++)
            {
                if (_inventoryReferences[i] != null)
                {
                    if (_inventoryReferences[i]!.Item != null)
                        AddItem(i, _inventoryReferences[i]!);
                    else
                        RemoveItem(_inventoryReferences[i]!);
                }
            }
        }

        public int[] GetHotbarIndexes(InventorySlot slot)
        {
            List<int> indices = new List<int>();

            // Check at which indices current slot is
            for (int i = 0; i < _inventoryReferences !.Length; i++)
            {
                if (_inventoryReferences[i] == slot)
                {
                    indices.Add(i);
                }
            }

            return indices.ToArray();
        }

        public void UnloadHotbar()
        {
            // Unload all hotbar entities
            Manager.GetInstance().RemoveEntities(_hotbarEntities);
            _hotbarEntities.Clear();
        }
    }
}
