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

namespace Villeon.GUI
{
    public class PlayerExpBar
    {
        private IEntity _frame = new Entity("emptyFrame");
        private IEntity _expFilling = new Entity("emptyHealthFilling");
        private IEntity _background = new Entity("emptyBackground");
        private List<IEntity> _level = new List<IEntity>();

        private float _width;
        private float _maxWidth;
        private float _height;

        private float _scale = 0.25f;
        private Vector2 _position;

        public PlayerExpBar()
        {
            Sprite sprite = Asset.GetSprite("GUI.Frame.png", SpriteLayer.ScreenGuiForeground, false);
            _width = _maxWidth = sprite.Width;
            _height = sprite.Height;

            _position = new Vector2(-10, 4f - (_height * _scale));

            CreateFrame(_position);
            CreateFilling(_position, 0, _height);
            UpdateFillingSprite();
            CreateLevel();
            SpawnExpBar();
        }

        public void Update()
        {
            CreateLevel();
            UpdateFillingSprite();
        }

        private void UpdateFillingSprite()
        {
            // Update Healthfluid
            Sprite fillingSprite = _expFilling.GetComponent<Sprite>();
            float expToWidthConverter = Stats.GetInstance().RequiredExperience / _maxWidth;
            float currentExpInWidth = Stats.GetInstance().Experience / expToWidthConverter;
            fillingSprite.Width = currentExpInWidth;
        }

        private void CreateLevel()
        {
            // Remove existing level
            if (_level.Count > 0)
                Manager.GetInstance().RemoveEntities(_level);

            // Clear it
            _level.Clear();

            // Add the new Leveltext
            Text text = new Text("Level " + Stats.GetInstance().Level.ToString(), _position + new Vector2((_width * _scale) + 0.1f, 0f), "Alagard", SpriteLayer.ScreenGuiOverlayForeGround, 0f, 0f, 0.2f);
            foreach (IEntity entity in text.GetEntities())
            {
                _level.Add(entity);
            }

            // Spawn the Leveltext
            foreach (IEntity character in _level)
            {
               Manager.GetInstance().AddEntityToScene(character, "DungeonScene");
               Manager.GetInstance().AddEntityToScene(character, "BossScene");
            }
        }

        private void CreateFrame(Vector2 position)
        {
            _frame = new Entity(new Transform(position, _scale, 0f), "Player EXP Frame");
            Sprite frame = Asset.GetSprite("GUI.Frame.png", SpriteLayer.ScreenGuiOverlayForeGround, true);

            _frame.AddComponent(frame);
        }

        private void CreateFilling(Vector2 position, float width, float height)
        {
            _expFilling = new Entity(new Transform(position, _scale, 0f), "Player EXP Filling");
            Sprite healthFilling = new Sprite(SpriteLayer.ScreenGuiOverlayMiddleGround, width, height, true);
            healthFilling.Color = Color4.Green;
            _expFilling.AddComponent(healthFilling);
        }

        private void CreateBackground(Vector2 position, float width, float height)
        {
            _background = new Entity(new Transform(position, _scale, 0f), "Player EXP Background");
            Sprite backgroundSprite = new Sprite(SpriteLayer.ScreenGuiOverlayBackground, width, height);
            backgroundSprite.Color = Color4.Black;
            _background.AddComponent(backgroundSprite);
        }

        private void SpawnExpBar()
        {
            Manager.GetInstance().AddEntity(_frame);
            Manager.GetInstance().AddEntity(_expFilling);
            Manager.GetInstance().AddEntity(_background);
        }
    }
}
