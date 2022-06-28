using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.Helper;
using Zenseless.OpenTK;

namespace Villeon.Utils
{
    public static class AnimationLoader
    {
        // Loads in animations sheet and return object of type animation
        public static Animation CreateAnimationFromFile(string filename, float animationLength)
        {
            // Load in texture from files
            Texture2D texture = Asset.GetTexture(filename);

            // Create new animation object with given length
            Animation animation = new Animation(animationLength);

            // Add Sprites to Animation
            SpriteSheet spriteSheet = Asset.GetSpriteSheet(filename);
            foreach (Sprite sprite in spriteSheet.Sprites)
            {
                animation.AnimationSprite.Add(sprite);
            }

            // Return finished animation
            return animation;
        }
    }
}
