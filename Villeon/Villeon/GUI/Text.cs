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
    public class Text
    {
        private List<IEntity> _letters;

        // Text settings
        private string _text;
        private Vector2 _position;
        private float _letterSpacing;
        private float _lineSpacing;
        private float _letterScale;

        public Text(string text, Vector2 position, string font, float letterSpacing = 1.1f, float lineSpacing = 1.1f, float letterScale = 1f)
        {
            _letters = new List<IEntity>();

            _text = text;
            _position = position;
            _letterSpacing = letterSpacing * letterScale;
            _lineSpacing = lineSpacing * letterScale;
            _letterScale = letterScale;

            CreateLetters(font, SpriteLayer.ScreenGuiForeground);
        }

        public Text(string text, Vector2 position, string font, SpriteLayer layer, float letterSpacing = 1.1f, float lineSpacing = 1.1f, float letterScale = 1f)
        {
            _letters = new List<IEntity>();

            _text = text;
            _position = position;
            _letterSpacing = letterSpacing * letterScale;
            _lineSpacing = lineSpacing * letterScale;
            _letterScale = letterScale;

            CreateLetters(font, layer);
        }

        public List<IEntity> Letters { get => _letters; set => _letters = value; }

        public IEntity[] GetEntities()
        {
            return _letters.ToArray();
        }

        private void CreateLetters(string fontName, SpriteLayer layer)
        {
            Vector2 letterPosition = _position;
            float spriteHeight = Font.FontHeight * _letterScale;
            foreach (char c in _text)
            {
                // Move to next line '\n'
                if (c == '\n')
                {
                    letterPosition.X = _position.X;
                    letterPosition.Y -= spriteHeight + _lineSpacing;
                    continue;
                }

                Sprite letterSprite = Font.GetCharacter(c, layer, false);
                float spriteWidth = letterSprite.Width * _letterScale;

                // Create Entity for letter
                Entity letterEntity = new Entity(new Transform(letterPosition, _letterScale, 0f), "TBX[" + c + "]");
                Sprite letterSpriteCopy = new Sprite(letterSprite);
                letterEntity.AddComponent(letterSpriteCopy);
                _letters.Add(letterEntity);

                // Set position for next letter
                letterPosition.X += spriteWidth + _letterSpacing;
            }
        }
    }
}
