using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Zenseless.OpenTK;

namespace Villeon.Assets
{
    public class SpriteSheet
    {
        private Texture2D _sheetTexture;
        private List<Sprite> _sprites;
        private float _spriteWidth;
        private float _spriteHeight;

        public SpriteSheet(Color4 color, Texture2D texture, int spriteWidth, int spriteHeight, int numSprites, int spacing = 0, SpriteLayer layer = SpriteLayer.Background)
        {
            _sprites = new List<Sprite>();
            _sheetTexture = texture;
            _spriteWidth = spriteWidth / 8f;
            _spriteHeight = spriteHeight / 8f;
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
                Sprite sprite = new Sprite(texture, layer, texCoords, spriteWidth, spriteHeight);
                Sprites.Add(sprite);

                // Go to next sprite
                currentX += spriteWidth + spacing;
                if (currentX >= texture.Width)
                {
                    currentX = 0;
                    currentY -= spriteHeight + spacing;
                }
            }
        }

        public List<Sprite> Sprites { get => _sprites; }

        public float SpriteWidth { get => _spriteWidth; }

        public float SpriteHeight { get => _spriteHeight; }

        public Sprite GetSprite(int index)
        {
            return Sprites[index];
        }

        public Sprite GetSprite(int index, SpriteLayer layer)
        {
            Sprites[index].RenderLayer = layer;
            return Sprites[index];
        }

        public Sprite GetSprite(int index, SpriteLayer layer, bool isDynamic)
        {
            Sprites[index].RenderLayer = layer;
            Sprites[index].IsDynamic = isDynamic;
            return Sprites[index];
        }
    }
}
