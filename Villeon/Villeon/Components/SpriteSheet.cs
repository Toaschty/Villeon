using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Render;
using Zenseless.OpenTK;

namespace Villeon.Components
{
    public class SpriteSheet
    {
        private Texture2D _sheetTexture;
        private List<Sprite> _sprites;

        public SpriteSheet(Color4 color, Texture2D texture, int spriteWidth, int spriteHeight, int numSprites, int spacing = 0, SpriteLayer layer = SpriteLayer.Background)
        {
            _sprites = new List<Sprite>();
            _sheetTexture = texture;
            int currentX = 0;
            int currentY = texture.Height - spriteHeight;

            for (int i = 0; i < numSprites; i++)
            {
                // Coordinates normalized
                float topY = (currentY + spriteHeight) / (float)texture.Height;
                float rightX = (currentX + spriteWidth) / (float)texture.Width;
                float leftX = currentX / (float)texture.Width;
                float bottomY = currentY / (float)texture.Height;

                Vector2[] texCoords = new Vector2[4]
                {
                    new Vector2(leftX, bottomY),        // UV: Bottom left
                    new Vector2(rightX, bottomY),       // UV: Bottom right
                    new Vector2(leftX, topY),           // UV: Top left
                    new Vector2(rightX, topY),          // UV: Top right
                };
                Sprite sprite = new Sprite(Color4.White, texture, layer, texCoords);
                _sprites.Add(sprite);

                // Go to next sprite
                currentX += spriteWidth + spacing;
                if (currentX >= texture.Width)
                {
                    currentX = 0;
                    currentY -= spriteHeight + spacing;
                }
            }
        }

        public Sprite GetSprite(int index)
        {
            return _sprites[index];
        }

        public Sprite GetSprite(int index, SpriteLayer layer)
        {
            _sprites[index].RenderLayer = layer;
            return _sprites[index];
        }

        public Sprite GetSprite(int index, SpriteLayer layer, bool isDynamic)
        {
            _sprites[index].RenderLayer = layer;
            _sprites[index].IsDynamic = isDynamic;
            return _sprites[index];
        }
    }
}
