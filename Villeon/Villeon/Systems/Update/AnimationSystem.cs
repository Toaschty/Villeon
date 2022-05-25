using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Utils;

namespace Villeon.Systems.Update
{
    public class AnimationSystem : System, IUpdateSystem
    {
        public AnimationSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(AnimationController));
            Signature = Signature.AddToSignature(typeof(Sprite));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                AnimationController animation = entity.GetComponent<AnimationController>();
                Sprite sprite = entity.GetComponent<Sprite>();

                // Increase elapsed time in current animation
                Animation currentAnim = animation.GetSelectedAnimation();

                currentAnim.ElapsedTime += time;

                // Check if frame switch is necessary
                if (currentAnim.ElapsedTime > currentAnim.FrameDuration)
                {
                    // Reset timer
                    currentAnim.ElapsedTime = 0;

                    // Increase current frame index
                    currentAnim.CurrentFrame++;

                    // Set Texture and Texture Coordinates to new frame
                    sprite.Texture = currentAnim.AnimationSprite[currentAnim.CurrentFrame].Texture;
                    sprite.TexCoords = currentAnim.AnimationSprite[currentAnim.CurrentFrame].TexCoords;
                    sprite.Width = currentAnim.AnimationSprite[currentAnim.CurrentFrame].Width;
                    sprite.Height = currentAnim.AnimationSprite[currentAnim.CurrentFrame].Height;
                }
            }
        }
    }
}
