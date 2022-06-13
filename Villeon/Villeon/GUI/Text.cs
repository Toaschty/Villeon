using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;
using Villeon.Render;

namespace Villeon.GUI
{
    public class Text
    {
        private List<Entity> _letters;

        // Text settings
        private string _text;
        private Vector2 _position;
        private float _letterSpacing;
        private float _lineSpacing;
        private float _letterScale;

        public Text(string text, Vector2 position, string font, float letterSpacing = 1.1f, float lineSpacing = 1.1f, float letterScale = 1f)
        {
            _letters = new List<Entity>();

            _text = text;
            _position = position;
            _letterSpacing = letterSpacing * letterScale;
            _lineSpacing = lineSpacing * letterScale;
            _letterScale = letterScale;

            CreateLetters(font);
        }

        public Entity[] GetEntities()
        {
            return _letters.ToArray();
        }

        private void CreateLetters(string fontName)
        {
            Font font = new Font(Color4.White, Assets.GetTexture("Fonts." + fontName + ".png"), "Fonts." + fontName + ".json");

            Vector2 letterPosition = _position;
            float spriteHeight = font.FontHeight * _letterScale;
            foreach (char c in _text)
            {
                // Move to next line '\n'
                if (c == '\n')
                {
                    letterPosition.X = _position.X;
                    letterPosition.Y -= spriteHeight + _lineSpacing;
                    continue;
                }

                Sprite letterSprite = font.GetCharacter(c, SpriteLayer.ScreenGuiForeground, false);
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
