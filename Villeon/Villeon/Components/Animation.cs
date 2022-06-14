using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Animation
    {
        // Animation variables
        private List<Sprite> _animationSprites;
        private int _currentFrame;
        private float _frameDuration;
        private float _elapsedTime;

        public Animation(float frameDuration)
        {
            _animationSprites = new List<Sprite>();
            _frameDuration = frameDuration;
            _currentFrame = 0;
            _elapsedTime = 0;
        }

        public List<Sprite> AnimationSprite
        {
            get { return _animationSprites; }
            set { _animationSprites = value; }
        }

        public float FrameDuration
        {
            get { return _frameDuration; }
            set { _frameDuration = value; }
        }

        public float ElapsedTime
        {
            get { return _elapsedTime; }
            set { _elapsedTime = value; }
        }

        public int CurrentFrame
        {
            get { return _currentFrame; }

            // Loop back to the first frame if last frame + 1 is reached
            set { _currentFrame = _currentFrame >= _animationSprites.Count - 1 ? 0 : value; }
        }
    }
}
