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
        private float _shadowHeight;

        public Text(string text, Vector2 position, float letterSpacing = 1.1f, float lineSpacing = 1.1f, float letterScale = 1f, float shadowHeight = -0.01f)
        {
            _letters = new List<Entity>();

            _text = text;
            _position = position;
            _letterSpacing = letterSpacing * letterScale;
            _lineSpacing = lineSpacing * letterScale;
            _letterScale = letterScale;
            _shadowHeight = shadowHeight;

            CreateLetters();
        }

        public Entity[] GetEntities()
        {
            return _letters.ToArray();
        }

        private void CreateLetters()
        {
            SpriteSheet fontSheet = Assets.GetSpriteSheet("Fonts.VilleonFont.png");
            Sprite letterSprite;

            Vector2 letterPosition = _position;

            foreach (char c in _text)
            {
                // Move to next line '\n'
                if (c == '\n')
                {
                    letterPosition.X = _position.X;
                    letterPosition.Y -= _lineSpacing;
                    continue;
                }

                letterSprite = fontSheet.GetSprite(c - ' ', SpriteLayer.ScreenGuiForeground, false);

                // Create shadow Entity
                Entity shadowEntity = new Entity(new Transform(letterPosition + new Vector2(0f, _shadowHeight), _letterScale, 0f), "TBX[" + c + "]");
                Sprite letterShadowSpriteCopy = new Sprite(letterSprite);
                letterShadowSpriteCopy.Color = Color4.Black;
                shadowEntity.AddComponent(letterShadowSpriteCopy);
                _letters.Add(shadowEntity);

                // Create Entity for letter
                Entity letterEntity = new Entity(new Transform(letterPosition, _letterScale, 0f), "TBX[" + c + "]");
                Sprite letterSpriteCopy = new Sprite(letterSprite);
                letterSpriteCopy.Color = Color4.White;
                letterEntity.AddComponent(letterSpriteCopy);
                _letters.Add(letterEntity);

                // Set position for next letter
                letterPosition.X += _letterSpacing;
            }
        }
    }
}
