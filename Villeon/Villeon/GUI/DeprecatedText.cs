﻿using System;
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
    public class DeprecatedText
    {
        private static SpriteSheet _fontSheet = Asset.GetSpriteSheet("HenksFont.png");
        private static Dictionary<IEntity, List<IEntity>> _textEntities = new Dictionary<IEntity, List<IEntity>>();

        public static void Write(IEntity entity, string text, Vector2 offset, float scale)
        {
            // Add the entity wich holds the text entities
            _textEntities.Add(entity, new List<IEntity>());

            // Add each letters to the list
            Transform transform = entity.GetComponent<Transform>();
            Vector2 textPosition = transform.Position + offset;
            foreach (char c in text.ToCharArray())
            {
                if (c == '\n')
                {
                    textPosition.Y += -1.1f * scale;
                    textPosition.X = 0f;
                    continue;
                }

                //Transform trans = new Transform(transform.Position, transform.Scale, transform.Degrees);
                IEntity letter = new Entity(transform, c.ToString());

                letter.AddComponent(_fontSheet.GetSprite(c - ' ', SpriteLayer.Foreground, true));
                _textEntities[entity].Add(letter);
                Manager.GetInstance().AddEntity(letter);

                textPosition.X += 1.1f * scale;
            }
        }

        public void Overwrite(IEntity entity, string newText)
        {
            // Overwrites old text
        }

        public void Clear(IEntity entity)
        {
            // Deletes
        }
    }
}
