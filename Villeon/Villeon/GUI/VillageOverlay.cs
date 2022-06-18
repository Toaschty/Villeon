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
    public class VillageOverlay : IOverlay
    {
        private List<IEntity> _entities;

        public VillageOverlay()
        {
            // Create overlay layout
            _entities = new List<IEntity>();

            // Load Sprites
            Sprite dungeonButton = Asset.GetSprite("GUI.Dungeon_Button.png", SpriteLayer.ScreenGuiBackground, false);
            Sprite mapButton = Asset.GetSprite("GUI.Map_Button.png", SpriteLayer.ScreenGuiBackground, false);
            Sprite equipmentButton = Asset.GetSprite("GUI.Equipment_Button.png", SpriteLayer.ScreenGuiBackground, false);
            Sprite inventoryButton = Asset.GetSprite("GUI.Inventar_Button.png", SpriteLayer.ScreenGuiBackground, false);
            Sprite hotbarIcon = Asset.GetSprite("GUI.Slot.png", SpriteLayer.ScreenGuiBackground, false);

            // Menu Button Entities
            IEntity dungeonButtonEntity = new Entity(new Transform(new Vector2(-9f, -5f), 0.3f, 0f), "Dungeon Button");
            dungeonButtonEntity.AddComponent(dungeonButton);
            _entities.Add(dungeonButtonEntity);

            IEntity mapButtonEntity = new Entity(new Transform(new Vector2(-7.5f, -5f), 0.3f, 0f), "Map Button");
            mapButtonEntity.AddComponent(mapButton);
            _entities.Add(mapButtonEntity);

            IEntity equipmentButtonEntity = new Entity(new Transform(new Vector2(-6f, -5f), 0.3f, 0f), "Equipment Button");
            equipmentButtonEntity.AddComponent(equipmentButton);
            _entities.Add(equipmentButtonEntity);

            IEntity inventoryButtonEntity = new Entity(new Transform(new Vector2(-4.5f, -5f), 0.3f, 0f), "Inventory Button");
            inventoryButtonEntity.AddComponent(inventoryButton);
            _entities.Add(inventoryButtonEntity);

            // Hotbar Entities
            float offset = hotbarIcon.Width * 0.3f;

            IEntity hotbarSlot1 = new Entity(new Transform(new Vector2(4.5f - offset, -5f), 0.3f, 0f), "Slot 1");
            hotbarSlot1.AddComponent(hotbarIcon);
            _entities.Add(hotbarSlot1);

            Text slot1Text = new Text("1", new Vector2(4.3f, -5.1f), "Alagard", SpriteLayer.ScreenGuiBackground, 1f, 1f, 0.3f);
            _entities.AddRange(slot1Text.GetEntities());

            IEntity hotbarSlot2 = new Entity(new Transform(new Vector2(6f - offset, -5f), 0.3f, 0f), "Slot 2");
            hotbarSlot2.AddComponent(hotbarIcon);
            _entities.Add(hotbarSlot2);

            Text slot2Text = new Text("2", new Vector2(5.8f, -5.1f), "Alagard", SpriteLayer.ScreenGuiBackground, 1f, 1f, 0.3f);
            _entities.AddRange(slot2Text.GetEntities());

            IEntity hotbarSlot3 = new Entity(new Transform(new Vector2(7.5f - offset, -5f), 0.3f, 0f), "Slot 3");
            hotbarSlot3.AddComponent(hotbarIcon);
            _entities.Add(hotbarSlot3);

            Text slot3Text = new Text("3", new Vector2(7.2f, -5.1f), "Alagard", SpriteLayer.ScreenGuiBackground, 1f, 1f, 0.3f);
            _entities.AddRange(slot3Text.GetEntities());

            IEntity hotbarSlot4 = new Entity(new Transform(new Vector2(9f - offset, -5f), 0.3f, 0f), "Slot 4");
            hotbarSlot4.AddComponent(hotbarIcon);
            _entities.Add(hotbarSlot4);

            Text slot4Text = new Text("4", new Vector2(8.8f, -5.1f), "Alagard", SpriteLayer.ScreenGuiBackground, 1f, 1f, 0.3f);
            _entities.AddRange(slot4Text.GetEntities());
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }
    }
}
