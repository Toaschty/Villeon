using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;
using Zenseless.OpenTK;

namespace Villeon.Helper
{
    public class TextWriter
    {
        private static SpriteSheet _fontSheet = Assets.GetSpriteSheet("HenksFont.png");
        private static List<IEntity> _entities = new List<IEntity>();

        public static void Write(string text, Vector2 worldPosition)
        {
            Vector2 textPosition = worldPosition;
            foreach (char c in text.ToCharArray())
            {
                if (c == '\n')
                {
                    textPosition.Y += -1.1f;
                    textPosition.X = 0f;
                    continue;
                }

                IEntity letter = new Entity(new Transform(textPosition, 1f, 0f), c.ToString());
                letter.AddComponent(_fontSheet.GetSprite(c - ' ', Render.SpriteLayer.Foreground, true));
                _entities.Add(letter);
                Manager.GetInstance().AddEntity(letter);

                textPosition.X += 1.1f;
            }
        }

        public static void RemoveText()
        {
            foreach (IEntity entity in _entities)
            {
                Manager.GetInstance().RemoveEntity(entity);
            }
        }
    }
}
