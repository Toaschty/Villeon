using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;

namespace Villeon.Systems.Update
{
    public class AnimatedTileSystem : System, IUpdateSystem
    {
        public AnimatedTileSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(AnimatedTile));
        }

        public void Update(float time)
        {
            foreach (var entity in Entities)
            {
                AnimatedTile animTile = entity.GetComponent<AnimatedTile>();

                animTile.ElapsedTime += time;

                if (animTile.ElapsedTime > animTile.FrameDuration)
                {
                    animTile.CurrentFrame++;

                    if (animTile.CurrentFrame >= animTile.AnimationFrames.Count)
                        animTile.CurrentFrame = 0;

                    animTile.ElapsedTime = 0;
                }
            }
        }
    }
}
