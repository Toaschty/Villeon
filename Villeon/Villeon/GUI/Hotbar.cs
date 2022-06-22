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

        private List<IEntity> _hotbarEntities;

        private Hotbar()
        {
            _hotbarEntities = new List<IEntity>();
            _hotbarSlots = new InventorySlot[4];

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

        public void AddItem(int index, InventorySlot itemSlot)
        {
            // Add item to slot
            _hotbarSlots[index].Item = ItemLoader.GetItem(itemSlot.Item !.Name);
            _hotbarSlots[index].SetStack(itemSlot.Count);

            // Adjust RenderLayer for each entity
            foreach (IEntity textEntity in _hotbarSlots[index].ItemEntites)
            {
                textEntity.GetComponent<Sprite>().RenderLayer = SpriteLayer.ScreenGuiOverlayForeGround;
            }

            _hotbarEntities.AddRange(_hotbarSlots[index].ItemEntites);

            ReloadHotbar();
        }

        private void ReloadHotbar()
        {
            Manager.GetInstance().RemoveEntities(_hotbarEntities);
            Manager.GetInstance().AddEntities(_hotbarEntities.ToArray());
        }

        // Use item on current slot
        public void UseItemInHotbar(int index)
        {
            if (_hotbarSlots[index].Item == null)
                return;

            // Remove all item entities from hotbar
            foreach (IEntity entity in _hotbarSlots[index].ItemEntites)
            {
                _hotbarEntities.Remove(entity);
            }

            // Decrease stack by one
            _hotbarSlots[index].DecreaseStack();

            // If stack reached 0 -> Delete item from hotbar
            if (_hotbarSlots[index].Count == 0)
            {
                RemoveItem(index);
                return;
            }

            // Add item entities to hotbar
            _hotbarEntities.AddRange(_hotbarSlots[index].ItemEntites);

            // Reload hotbar entities
            ReloadHotbar();
        }

        private void RemoveItem(int index)
        {
            // Delete all item entities from Scene
            Manager.GetInstance().RemoveEntities(_hotbarSlots[index].ItemEntites);
            _hotbarSlots[index].Item = null;
        }
    }
}
