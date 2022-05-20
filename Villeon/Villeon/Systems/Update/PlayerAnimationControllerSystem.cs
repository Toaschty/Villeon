using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class PlayerAnimationControllerSystem : System, IUpdateSystem
    {
        public PlayerAnimationControllerSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(AnimationController));
            Signature = Signature.AddToSignature(typeof(Sprite));
            Signature = Signature.AddToSignature(typeof(Player));
        }

        public void Update(float time)
        {
            // Get current walking direction
            float leftRightAxis = KeyHandler.IsPressed(Keys.A) ? -1 : KeyHandler.IsPressed(Keys.D) ? 1 : 0;
            float topDownAxis = KeyHandler.IsPressed(Keys.S) ? -1 : KeyHandler.IsPressed(Keys.W) ? 1 : 0;

            foreach (IEntity entity in Entities)
            {
                // Get animation controller
                AnimationController controller = entity.GetComponent<AnimationController>();

                // Default idle animation
                controller.SetAnimation(0);

                // Depending on direction -> Choose right animation
                if (topDownAxis < 0)
                    controller.SetAnimation(2);
                else if (topDownAxis > 0)
                    controller.SetAnimation(1);

                if (leftRightAxis < 0)
                    controller.SetAnimation(3);
                else if (leftRightAxis > 0)
                    controller.SetAnimation(4);
            }
        }
    }
}
