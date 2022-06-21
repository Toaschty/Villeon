using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.GUI
{
    public class PlayerHealthBar
    {
        private IEntity _frame = new Entity("emptyFrame");
        private IEntity _healthFilling = new Entity("emptyHealthFilling");
        private IEntity _background = new Entity("emptyBackground");

        private float _width;
        private float _maxWidth;
        private float _height;

        private float _oldHealth;
        private float _maxHealth;

        private float _scale = 0.4f;

        public PlayerHealthBar(int maxHealth)
        {
            Sprite sprite = Asset.GetSprite("GUI.Frame.png", SpriteLayer.ScreenGuiForeground, false);
            _width = _maxWidth = sprite.Width;
            _height = sprite.Height;

            Vector2 position = new Vector2(-10, 5f - (_height * _scale));
            _maxHealth = maxHealth;

            CreateFrame(position);
            //CreateBackground(position, _width, _height);
            CreateFilling(position, _width, _height);

            SpawnHealthBar();
        }

        public void UpdateHealthbar(float newHealth)
        {
            CalculateHealth(newHealth);
        }

        private void CalculateHealth(float newHealth)
        {
            float factor = newHealth / _oldHealth;
            _width *= factor;

            _oldHealth = newHealth;

            // Reset the health
            if (newHealth == _maxHealth)
                _width = _maxWidth;

            Sprite sprite = _healthFilling.GetComponent<Sprite>() !;
            sprite.Width = _width;
        }

        private void CreateFrame(Vector2 position)
        {
            _frame = new Entity(new Transform(position, _scale, 0f), "Player Health Frame");
            Sprite frame = Asset.GetSprite("GUI.Frame.png", SpriteLayer.ScreenGuiForeground, false);

            _frame.AddComponent(frame);
        }

        private void CreateFilling(Vector2 position, float width, float height)
        {
            _healthFilling = new Entity(new Transform(position, _scale, 0f), "Player Health Filling");
            Sprite healthFilling = new Sprite(SpriteLayer.ScreenGuiMiddleground, width, height, true);
            healthFilling.Color = Color4.Red;
            _healthFilling.AddComponent(healthFilling);
        }

        private void CreateBackground(Vector2 position, float width, float height)
        {
            _background = new Entity(new Transform(position, _scale, 0f), "Player Health Background");
            Sprite backgroundSprite = new Sprite(SpriteLayer.ScreenGuiBackground, width, height);
            backgroundSprite.Color = Color4.Black;
            _background.AddComponent(backgroundSprite);
        }

        private void SpawnHealthBar()
        {
            Manager.GetInstance().AddEntityToScene(_frame, "DungeonScene");
            Manager.GetInstance().AddEntityToScene(_healthFilling, "DungeonScene");
            Manager.GetInstance().AddEntityToScene(_background, "DungeonScene");
        }
    }
}