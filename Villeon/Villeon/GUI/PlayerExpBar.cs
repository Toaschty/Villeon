using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;

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
        private float _maxExp;

        private float _scale = 0.25f;
        private Vector2 _position;

        private Exp _exp;

        public PlayerExpBar(Exp exp)
        {
            _exp = exp;

            Sprite sprite = Asset.GetSprite("GUI.Frame.png", SpriteLayer.ScreenGuiForeground, false);
            _width = _maxWidth = sprite.Width;
            _height = sprite.Height;

            _position = new Vector2(-10, 4f - (_height * _scale));
            _maxExp = _exp.MaxExp;

            CreateFrame(_position);
            //CreateBackground(_position, _width, _height);
            CreateFilling(_position, 0, _height);
            CreateLevel();
            SpawnExpBar();
        }

        public void UpdateExpbar(int expGain)
        {
            GainExp(expGain);
        }

        private void GainExp(int expGain)
        {
            bool levelUp = _exp.GainExp(expGain);
            if (levelUp)
                CreateLevel();

            Console.WriteLine("Current EXP: " + _exp.CurrentExp);
            Console.WriteLine("Max EXP: " + _exp.MaxExp);
            Console.WriteLine("Level: " + _exp.Level);

            // Update Healthfluid
            Sprite fillingSprite = _expFilling.GetComponent<Sprite>();
            float expToWidthConverter = _exp.MaxExp / _maxWidth;
            float currentExpInWidth = _exp.CurrentExp / expToWidthConverter;
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
            Text text = new Text("Level " + _exp.Level.ToString(), _position + new Vector2((_width * _scale) + 0.1f, 0f), "Alagard", SpriteLayer.ScreenGuiForeground, 0f, 0f, 0.2f);
            foreach (IEntity entity in text.GetEntities())
            {
                _level.Add(entity);
            }

            // Spawn the Leveltext
            foreach (IEntity character in _level)
            {
               Manager.GetInstance().AddEntityToScene(character, "DungeonScene");
            }
        }

        private void CreateFrame(Vector2 position)
        {
            _frame = new Entity(new Transform(position, _scale, 0f), "Player EXP Frame");
            Sprite frame = Asset.GetSprite("GUI.Frame.png", SpriteLayer.ScreenGuiForeground, true);

            _frame.AddComponent(frame);
        }

        private void CreateFilling(Vector2 position, float width, float height)
        {
            _expFilling = new Entity(new Transform(position, _scale, 0f), "Player EXP Filling");
            Sprite healthFilling = new Sprite(SpriteLayer.ScreenGuiMiddleground, width, height, true);
            healthFilling.Color = Color4.Green;
            _expFilling.AddComponent(healthFilling);
        }

        private void CreateBackground(Vector2 position, float width, float height)
        {
            _background = new Entity(new Transform(position, _scale, 0f), "Player EXP Background");
            Sprite backgroundSprite = new Sprite(SpriteLayer.ScreenGuiBackground, width, height);
            backgroundSprite.Color = Color4.Black;
            _background.AddComponent(backgroundSprite);
        }

        private void SpawnExpBar()
        {
            Manager.GetInstance().AddEntityToScene(_frame, "DungeonScene");
            Manager.GetInstance().AddEntityToScene(_expFilling, "DungeonScene");
            Manager.GetInstance().AddEntityToScene(_background, "DungeonScene");

        }
    }
}
