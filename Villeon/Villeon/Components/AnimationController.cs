using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Utils;

namespace Villeon.Components
{
    public class AnimationController : IComponent
    {
        // List of all possible animations
        private List<Animation> _animations;

        // Index of current animation
        private int _selection;

        public AnimationController()
        {
            _animations = new List<Animation>();
            _selection = 0;
        }

        // Set current animation to play
        public void setAnimation(int index)
        {
            _selection = index;
        }

        // Add animation to the intenal animation list
        public void addAnimation(Animation animation)
        {
            _animations.Add(animation);
        }

        // Get activated animation
        public Animation getSelectedAnimation()
        {
            return _animations[_selection];
        }
    }
}
