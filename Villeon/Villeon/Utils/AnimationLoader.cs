using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.Helper;
using Zenseless.OpenTK;

namespace Villeon.Utils
{
    public static class AnimationLoader
    {
        // Loads in animations sheet and return object of type animation
        public static Animation CreateAnimationFromFile(String filename, int columns, int frameWidthInPixels, int frameHeightInPixels, float animationLength)
        {
            // Load in texture from files
            Texture2D texture = Assets.GetTexture(filename);

            // Create new animation object with given length
            Animation animation = new Animation(animationLength);

            // Calculate texture coordinates for each frame
            float frameWidth = 1 / columns;

            for (int x = 0; x < columns; x++)
            {
                Vector2[] texCoords = new Vector2[4]
                {
                    new Vector2(x * frameWidth, 0),
                    new Vector2((x * frameWidth) + frameWidth, 0),
                    new Vector2(x * frameWidth, 1),
                    new Vector2((x * frameWidth) + frameWidth, 1),
                };

                // Create new sprite with calculated data and add it to the animation
                Sprite frame = new Sprite(texture, Render.SpriteLayer.Foreground, texCoords, columns * frameWidthInPixels, frameHeightInPixels, true);
                animation.AnimationSprite.Add(frame);
            }

            // Return finished animation
            return animation;
        }
    }
}
