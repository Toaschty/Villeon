using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class AnimatedTile : Tile
    {
        public AnimatedTile(float x, float y, TileSetStruct tileset)
            : base(x, y, tileset)
        {
        }

        public AnimatedTile(float x, float y, TileSetStruct tileset, List<Box2> colliders)
            : base(x, y, tileset, colliders)
        {
        }

        public AnimatedTile(float x, float y, TileSetStruct tileset, List<Box2> colliders, List<int> frames, float duration)
            : base(x, y, tileset, colliders)
        {
            AnimationFrames = frames;
            FrameDuration = duration;
        }

        public List<int> AnimationFrames { get; set; } = new List<int>();

        public uint GetCurrentFrameID() => (uint)AnimationFrames[CurrentFrame];

        public override object Clone() => new AnimatedTile(_x, _y, TileSet, Colliders, AnimationFrames, FrameDuration);

        public int CurrentFrame = 0;
        public float FrameDuration = 0;
        public float ElapsedTime = 0;
    }
}
