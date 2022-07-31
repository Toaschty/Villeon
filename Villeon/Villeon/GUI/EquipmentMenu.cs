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
        private static EquipmentMenu? _instance;

        private List<IEntity> _entities;
        private List<IEntity> _statEntities;

        private float _letterScale = 0.35f;
        private float _letterScaleStats = 0.25f;

        private dynamic _charakterJson;

        // Equipped Sword / Shield
        private Item? _sword;
        private IEntity? _swordEntity;
        private Item? _shield;
        private IEntity? _shieldEntity;

        // Equipped Items
        private Item?[] _slots;
        private IEntity[] _slotsEntities;

        private EquipmentMenu()
        {
            // Create Menu layout
            _entities = new List<IEntity>();
            _statEntities = new List<IEntity>();

            // Hotbar slots
            _slots = new Item[4];
            _slotsEntities = new IEntity[4];

            // Load character data
            _charakterJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.Character.json")) !;

            // Load Sprites
            Sprite backgroundScrollSprite = Asset.GetSprite("GUI.Scrolls.Scroll_Equipment.png", SpriteLayer.ScreenGuiBackground, false);
            Sprite statHealthSprite = Asset.GetSprite("GUI.Equipment.Equipment_Stat_Health.png", SpriteLayer.ScreenGuiForeground, false);
            Sprite statAttackSprite = Asset.GetSprite("GUI.Equipment.Equipment_Stat_Attack.png", SpriteLayer.ScreenGuiForeground, false);
            Sprite statDefenseSprite = Asset.GetSprite("GUI.Equipment.Equipment_Stat_Defense.png", SpriteLayer.ScreenGuiForeground, false);
            Sprite swordSlotSprite = Asset.GetSprite("GUI.Inventory.InventorySlot_Sword.png", SpriteLayer.ScreenGuiMiddleground, false);
            Sprite shieldSlotSprite = Asset.GetSprite("GUI.Inventory.InventorySlot_Shield.png", SpriteLayer.ScreenGuiMiddleground, false);
            Sprite slotSprite = Asset.GetSprite("GUI.Inventory.InventorySlot.png", SpriteLayer.ScreenGuiMiddleground, false);

            // Background
            Vector2 scrollMiddle = new Vector2(backgroundScrollSprite.Width / 2f, backgroundScrollSprite.Height / 2f);
            IEntity backgroundImage = new Entity(new Transform(Vector2.Zero - (scrollMiddle * 0.5f), 0.5f, 0f), "BackgroundImage");
            backgroundImage.AddComponent(backgroundScrollSprite);
            _entities.Add(backgroundImage);

            // Character name
            string charakterName = _charakterJson.name.ToString();
            Text name = new Text(charakterName, new Vector2(-5.9f, 3.2f), "Alagard", 0f, 0.5f, _letterScale);
            _entities.AddRange(name.GetEntities());

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

            // Player stats
            Entity healthIcon = new Entity(new Transform(new Vector2(-6.2f, -4f), 0.2f, 0f), "Health Icon");
            healthIcon.AddComponent(statHealthSprite);
            _entities.Add(healthIcon);

            Text health = new Text(Stats.GetInstance().GetMaxHealth().ToString(), new Vector2(-5.5f, -4.1f), "Alagard", 0f, 0.5f, _letterScaleStats);
            Array.ForEach(health.GetEntities(), entity => _statEntities.Add(entity));

            Entity attackIcon = new Entity(new Transform(new Vector2(-4.2f, -4f), 0.2f, 0f), "Attack Icon");
            attackIcon.AddComponent(statAttackSprite);
            _entities.Add(attackIcon);

            Text attack = new Text(Stats.GetInstance().GetAttack().ToString(), new Vector2(-3.5f, -4.1f), "Alagard", 0f, 0.5f, _letterScaleStats);
            Array.ForEach(attack.GetEntities(), entity => _statEntities.Add(entity));

            Entity defenseIcon = new Entity(new Transform(new Vector2(-2.2f, -4f), 0.2f, 0f), "Defense Icon");
            defenseIcon.AddComponent(statDefenseSprite);
            _entities.Add(defenseIcon);

            Text defense = new Text(Stats.GetInstance().GetDefense().ToString(), new Vector2(-1.5f, -4.1f), "Alagard", 0f, 0.5f, _letterScaleStats);
            Array.ForEach(defense.GetEntities(), entity => _statEntities.Add(entity));

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
            IEntity slot1 = new Entity(new Transform(new Vector2(0.6f, -3.2f), 0.3f, 0f), "Slot1");
            IEntity slot2 = new Entity(new Transform(new Vector2(2.1f, -3.2f), 0.3f, 0f), "Slot2");
            IEntity slot3 = new Entity(new Transform(new Vector2(3.6f, -3.2f), 0.3f, 0f), "Slot3");
            IEntity slot4 = new Entity(new Transform(new Vector2(5.1f, -3.2f), 0.3f, 0f), "Slot4");
            slot1.AddComponent(slotSprite);
            slot2.AddComponent(slotSprite);
            slot3.AddComponent(slotSprite);
            slot4.AddComponent(slotSprite);
            _entities.Add(slot1);
            _entities.Add(slot2);
            _entities.Add(slot3);
            _entities.Add(slot4);

            _entities.AddRange(_statEntities);
        }

        public static EquipmentMenu GetInstance()
        {
            if (_instance == null)
            {
                _instance = new EquipmentMenu();
            }

            return _instance;
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }

        public bool OnKeyReleased(Keys key)
        {
            // Refresh stat texts
            RefreshStatText();

            return true;
        }

        // Add attack weapon to display
        public void AddAttackWeapon(Item weapon)
        {
            _sword = weapon;
            UpdateWeapons();
        }

        // Add defense weapon to display
        public void AddDefenseWeapon(Item weapon)
        {
            _shield = weapon;
            UpdateWeapons();
        }

        // Add item to hotbar
        public void AddItemInHotbar(int index, Item item)
        {
            _slots[index] = item;

            UpdateEquippedItems();
        }

        // Remove item from hotbar
        public void RemoveItemInHotbar(int index)
        {
            _slots[index] = null;

            UpdateEquippedItems();
        }

        private void UpdateEquippedItems()
        {
            // Remove existing entities
            Manager.GetInstance().RemoveEntities(_slotsEntities.ToArray());
            foreach (IEntity entity in _slotsEntities)
            {
                _entities.Remove(entity);
            }

            // Add new entities
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null)
                {
                    _slotsEntities[i] = new Entity(new Transform(new Vector2(0.6f + (i * 1.5f), -3.2f), 0.3f, 0f), "Slot");
                    _slotsEntities[i].AddComponent(_slots[i]!.Sprite);
                    _entities.Add(_slotsEntities[i]);
                }
            }
        }

        private void UpdateWeapons()
        {
            // Remove existing entities
            Manager.GetInstance().RemoveEntity(_swordEntity!);
            Manager.GetInstance().RemoveEntity(_shieldEntity!);
            _entities.Remove(_swordEntity!);
            _entities.Remove(_shieldEntity!);

            // Add new entities
            if (_sword != null)
            {
                _swordEntity = new Entity(new Transform(new Vector2(2.1f, 1f), 0.3f, 0f), "Weapon Slot Item");
                _swordEntity.AddComponent(_sword.Sprite);
                _entities.Add(_swordEntity);
            }

            if (_shield != null)
            {
                _shieldEntity = new Entity(new Transform(new Vector2(3.6f, 1f), 0.3f, 0f), "Shield Slot");
                _shieldEntity.AddComponent(_shield.Sprite);
                _entities.Add(_shieldEntity);
            }
        }

        private void RefreshStatText()
        {
            // Delete old text
            Manager.GetInstance().RemoveEntities(_entities);
            for (int i = 0; i < _statEntities.Count; i++)
                _entities.Remove(_statEntities[i]);

            _statEntities.Clear();

            // Add stats texts
            Text health = new Text(Stats.GetInstance().GetMaxHealth().ToString(), new Vector2(-5.5f, -4.1f), "Alagard", 0f, 0.5f, _letterScaleStats);
            Array.ForEach(health.GetEntities(), entity => _statEntities.Add(entity));

            Text attack = new Text(Stats.GetInstance().GetAttack().ToString(), new Vector2(-3.5f, -4.1f), "Alagard", 0f, 0.5f, _letterScaleStats);
            Array.ForEach(attack.GetEntities(), entity => _statEntities.Add(entity));

            Text defense = new Text(Stats.GetInstance().GetDefense().ToString(), new Vector2(-1.5f, -4.1f), "Alagard", 0f, 0.5f, _letterScaleStats);
            Array.ForEach(defense.GetEntities(), entity => _statEntities.Add(entity));

            _entities.AddRange(_statEntities);
        }
    }
}
