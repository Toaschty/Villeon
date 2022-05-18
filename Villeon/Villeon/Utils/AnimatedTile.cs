using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Zenseless.OpenTK;

namespace Villeon.Utils
{
    public class AnimatedTile : Tile
    {
        private int _currentFrame = 0;
        private float _frameDuration = 0;
        private float _elapsedTime = 0;

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
            _frameDuration = duration;
        }

        public List<int> AnimationFrames { get; set; } = new List<int>();

        public int CurrentFrame => _currentFrame;

        public float FrameDuration
        {
            get
            {
                return _frameDuration;
            }

            set
            {
                _frameDuration = value;
            }
        }

        public float ElapsedTime => _elapsedTime;

        public uint GetCurrentFrameID() => (uint)AnimationFrames[_currentFrame];
    }
}
