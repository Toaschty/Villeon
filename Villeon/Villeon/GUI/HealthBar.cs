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
    public class HealthBar
    {
        private IEntity _frame = new Entity("emptyFrame");
        private IEntity _healthFilling = new Entity("emptyHealthFilling");
        private IEntity _background = new Entity("emptyBackground");

        private float _width;
        private float _maxWidth;
        private float _height;
        private float _scale = 0.3f; // Should be 0.5 * transformscale

        private float _maxHealth;

        private Transform _trackingTransform;
        private Vector2 _offset;

        public HealthBar(int maxHealth, ref Transform transform, float spriteWidth, float spriteHeight)
        {
            Sprite healthbarSprite = Asset.GetSprite("GUI.Frame.png", SpriteLayer.GUIForeground, true);
            _width = _maxWidth = healthbarSprite.Width;
            _height = healthbarSprite.Height;
            _maxHealth = maxHealth;
            _scale *= transform.Scale.X;
            _trackingTransform = transform;
            _offset = new Vector2((spriteWidth / 2f * transform.Scale.X) - (healthbarSprite.Width / 2f * _scale), spriteHeight * transform.Scale.Y);

            CreateFrame(_trackingTransform.Position);
            CreateBackground(_trackingTransform.Position, _width, _height);
            CreateFilling(_trackingTransform.Position, _width, _height);

            SpawnHealthBar();
        }

        public void Remove()
        {
            Manager.GetInstance().RemoveEntity(_frame);
            Manager.GetInstance().RemoveEntity(_healthFilling);
            Manager.GetInstance().RemoveEntity(_background);
        }

        public void Update(int currentHealth)
        {
            // Update Position
            MoveHealthBar(_offset + _trackingTransform.Position);

            // Update Healthfluid
            Sprite fillingSprite = _healthFilling.GetComponent<Sprite>();
            float healthToWidthConverter = _maxHealth / _maxWidth;
            float currentHealthInWidth = currentHealth / healthToWidthConverter;
            fillingSprite.Width = currentHealthInWidth;
        }

        private void MoveHealthBar(Vector2 newBoundPosition)
        {
            // Gotta do it how it was if using normal text.
            _healthFilling.GetComponent<Transform>().Position = newBoundPosition;
            _frame.GetComponent<Transform>().Position = newBoundPosition;
            _background.GetComponent<Transform>().Position = newBoundPosition;
        }

        private void CreateFrame(Vector2 position)
        {
            _frame = new Entity(new Transform(position, _scale, 0f), "Enemy Health Frame");
            Sprite frame = Asset.GetSprite("GUI.Frame.png", SpriteLayer.GUIForeground, true);
            _frame.AddComponent(frame);
        }

        private void CreateFilling(Vector2 position, float width, float height)
        {
            _healthFilling = new Entity(new Transform(position, _scale, 0f), "Enemy Health Filling");
            Sprite healthFilling = new Sprite(SpriteLayer.GUIMiddleground, width, height, true);
            healthFilling.Color = Color4.Red;
            _healthFilling.AddComponent(healthFilling);
        }

        private void CreateBackground(Vector2 position, float width, float height)
        {
            _background = new Entity(new Transform(position, _scale, 0f), "Enemy Health Background");
            Sprite backgroundSprite = new Sprite(SpriteLayer.GUIBackground, width, height, true);
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
