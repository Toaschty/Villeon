using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;

namespace Villeon.Systems.Update
{
    public class AnimationSystem : System, IUpdateSystem
    {
        public AnimationSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Animation));
            Signature = Signature.AddToSignature(typeof(Sprite));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                Animation animation = entity.GetComponent<Animation>();
                Sprite sprite = entity.GetComponent<Sprite>();

                // Increase elapsed time in current animation
                animation.ElapsedTime += time;

                // Check if frame switch is necessary
                if (animation.ElapsedTime > animation.FrameDuration)
                {
                    // Reset timer
                    animation.ElapsedTime = 0;

                    // Increase current frame index
                    animation.CurrentFrame++;

                    // Set Texture and Texture Coordinates to new frame
                    sprite.Texture = animation.AnimationSprite[animation.CurrentFrame].Texture;
                    sprite.TexCoords = animation.AnimationSprite[animation.CurrentFrame].TexCoords;
                }
            }
        }
    }
}
