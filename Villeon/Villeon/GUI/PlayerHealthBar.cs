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

        private float _health;
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
            CreateFilling(position, _width, _height);

            SpawnHealthBar();
        }

        public void UpdateMaxHealth(int maxHealth)
        {
            _maxHealth = maxHealth;
            CalculateHealth(_maxHealth);
        }

        public void UpdateHealthbar(float newHealth)
        {
            _health = newHealth;
            CalculateHealth(newHealth);
        }

        private void CalculateHealth(float newHealth)
        {
            // Update Healthfluid
            Sprite fillingSprite = _healthFilling.GetComponent<Sprite>();
            float healthToWidthConverter = _maxHealth / _maxWidth;
            float currentHealthInWidth = newHealth / healthToWidthConverter;
            fillingSprite.Width = currentHealthInWidth;
        }

        private void CreateFrame(Vector2 position)
        {
            _frame = new Entity(new Transform(position, _scale, 0f), "Player Health Frame");
            Sprite frame = Asset.GetSprite("GUI.Frame.png", SpriteLayer.ScreenGuiOverlayForeGround, true);

            _frame.AddComponent(frame);
        }

        private void CreateFilling(Vector2 position, float width, float height)
        {
            _healthFilling = new Entity(new Transform(position, _scale, 0f), "Player Health Filling");
            Sprite healthFilling = new Sprite(SpriteLayer.ScreenGuiOverlayMiddleGround, width, height, true);
            healthFilling.Color = Color4.Red;
            _healthFilling.AddComponent(healthFilling);
        }

        private void CreateBackground(Vector2 position, float width, float height)
        {
            _background = new Entity(new Transform(position, _scale, 0f), "Player Health Background");
            Sprite backgroundSprite = new Sprite(SpriteLayer.ScreenGuiOverlayBackground, width, height);
            backgroundSprite.Color = Color4.Black;
            _background.AddComponent(backgroundSprite);
        }

        private void SpawnHealthBar()
        {
            Manager.GetInstance().AddEntity(_frame);
            Manager.GetInstance().AddEntity(_healthFilling);
            Manager.GetInstance().AddEntity(_background);
        }
    }
}