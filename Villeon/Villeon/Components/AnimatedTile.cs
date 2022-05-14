using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Zenseless.OpenTK;

namespace Villeon.Components
{
    public class AnimatedTile : Tile
    {
        public AnimatedTile(float x, float y, TileSetStruct tileset, Texture2D texture)
            : base(x, y, tileset, texture)
        {
        }

        public AnimatedTile(float x, float y, TileSetStruct tileset, List<Box2> colliders, Texture2D texture)
            : base(x, y, tileset, colliders, texture)
        {
        }

        public AnimatedTile(float x, float y, TileSetStruct tileset, List<Box2> colliders, List<int> frames, float duration, Texture2D texture)
            : base(x, y, tileset, colliders, texture)
        {
            AnimationFrames = frames;
            FrameDuration = duration;
        }

        public List<int> AnimationFrames { get; set; } = new List<int>();

        public uint GetCurrentFrameID() => (uint)AnimationFrames[CurrentFrame];

        public override object Clone() => new AnimatedTile(_x, _y, TileSet, Colliders, AnimationFrames, FrameDuration, Texture2D);

        public int CurrentFrame = 0;
        public float FrameDuration = 0;
        public float ElapsedTime = 0;
    }
}
