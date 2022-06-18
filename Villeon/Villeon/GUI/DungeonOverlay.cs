using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.GUI
{
    public class DungeonOverlay : IOverlay
    {
        private List<IEntity> _entities;

        public DungeonOverlay()
        {
            // Create overlay layout
            _entities = new List<IEntity>();

            // Load Sprites
            Sprite equipmentButton = Asset.GetSprite("GUI.Equipment_Button.png", SpriteLayer.ScreenGuiForeground, false);
            Sprite inventoryButton = Asset.GetSprite("GUI.Inventar_Button.png", SpriteLayer.ScreenGuiForeground, false);

            // Button Entities
            IEntity equipmentButtonEntity = new Entity(new Transform(new Vector2(-9f, -5f), 0.3f, 0f), "Equipment Button");
            equipmentButtonEntity.AddComponent(equipmentButton);
            _entities.Add(equipmentButtonEntity);

            IEntity inventoryButtonEntity = new Entity(new Transform(new Vector2(-7.5f, -5f), 0.3f, 0f), "Inventory Button");
            inventoryButtonEntity.AddComponent(inventoryButton);
            _entities.Add(inventoryButtonEntity);
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }
    }
}
