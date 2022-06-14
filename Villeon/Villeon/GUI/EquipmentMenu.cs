using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.GUI
{
    public class EquipmentMenu : IGUIMenu
    {
        private List<Entity> _entities;

        private float _letterScale = 0.35f;

        private dynamic _charakterJson;

        public EquipmentMenu()
        {
            // Create Menu layout
            _entities = new List<Entity>();

            // Load character data
            _charakterJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.Character.json")) !;

            // Load Sprites
            Sprite backgroundScrollSprite = Asset.GetSprite("GUI.Scroll_Equipment.png", SpriteLayer.ScreenGuiBackground, false);
            Sprite swordSlotSprite = Asset.GetSprite("GUI.Inventory.InventorySlot_Sword.png", SpriteLayer.ScreenGuiMiddleground, false);
            Sprite shieldSlotSprite = Asset.GetSprite("GUI.Inventory.InventorySlot_Shield.png", SpriteLayer.ScreenGuiMiddleground, false);
            Sprite slotSprite = Asset.GetSprite("GUI.Inventory.InventorySlot.png", SpriteLayer.ScreenGuiMiddleground, false);

            // Background
            Vector2 scrollMiddle = new Vector2(backgroundScrollSprite.Width / 2f, backgroundScrollSprite.Height / 2f);
            Entity backgroundImage = new Entity(new Transform(Vector2.Zero - (scrollMiddle * 0.5f), 0.5f, 0f), "BackgroundImage");
            backgroundImage.AddComponent(backgroundScrollSprite);
            _entities.Add(backgroundImage);

            // Character name
            string charakterName = _charakterJson.name.ToString();
            Text name = new Text(charakterName, new Vector2(-5.9f, 3.2f), "Alagard", 0f, 0.5f, _letterScale);
            Array.ForEach(name.GetEntities(), entity => _entities.Add(entity));

            // Idle player animation
            Entity image = new Entity(new Transform(new Vector2(-4.2f, -1.6f), 0.7f, 0f), "Profil");

            // Load animation from file
            Animation idleAnimation = AnimationLoader.CreateAnimationFromFile("Animations.player_idle.png", 0.3f);

            // Create sprite with first frame as start image
            Sprite player = new Sprite(idleAnimation.AnimationSprite.First());
            player.IsDynamic = true;
            player.RenderLayer = SpriteLayer.ScreenGuiMiddleground;

            AnimationController controller = new AnimationController();
            controller.AddAnimation(idleAnimation);
            controller.SetAnimation(0);
            image.AddComponent(player);
            image.AddComponent(controller);
            _entities.Add(image);

            // Weapon
            Text weapon = new Text("Sword & Shield", new Vector2(0.6f, 3.2f), "Alagard", 0f, 0.5f, _letterScale);
            Array.ForEach(weapon.GetEntities(), entity => _entities.Add(entity));

            // Weapon Slot
            Entity weaponSlot = new Entity(new Transform(new Vector2(2.1f, 1f), 0.3f, 0f), "Weapon Slot");
            Entity shieldSlot = new Entity(new Transform(new Vector2(3.6f, 1f), 0.3f, 0f), "Shield Slot");
            weaponSlot.AddComponent(swordSlotSprite);
            shieldSlot.AddComponent(shieldSlotSprite);
            _entities.Add(weaponSlot);
            _entities.Add(shieldSlot);

            // Hot Bar
            Text hotbar = new Text("Equiped Items", new Vector2(0.85f, -1f), "Alagard", 0f, 0.5f, _letterScale);
            Array.ForEach(hotbar.GetEntities(), entity => _entities.Add(entity));

            // Hotbar Slots
            Entity slot1 = new Entity(new Transform(new Vector2(0.6f, -3.2f), 0.3f, 0f), "Slot1");
            Entity slot2 = new Entity(new Transform(new Vector2(2.1f, -3.2f), 0.3f, 0f), "Slot2");
            Entity slot3 = new Entity(new Transform(new Vector2(3.6f, -3.2f), 0.3f, 0f), "Slot3");
            Entity slot4 = new Entity(new Transform(new Vector2(5.1f, -3.2f), 0.3f, 0f), "Slot4");
            slot1.AddComponent(slotSprite);
            slot2.AddComponent(slotSprite);
            slot3.AddComponent(slotSprite);
            slot4.AddComponent(slotSprite);
            _entities.Add(slot1);
            _entities.Add(slot2);
            _entities.Add(slot3);
            _entities.Add(slot4);
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }

        public bool OnKeyReleased(Keys key)
        {
            return true;
        }
    }
}
