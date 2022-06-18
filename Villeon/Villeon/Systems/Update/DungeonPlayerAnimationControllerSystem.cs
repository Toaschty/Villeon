using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class DungeonPlayerAnimationControllerSystem : System, IUpdateSystem
    {
        public DungeonPlayerAnimationControllerSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(AnimationController), typeof(Sprite), typeof(Player), typeof(Physics));
        }

        public void Update(float time)
        {
            // Get current walking direction
            float leftRightAxis = KeyHandler.IsHeld(Keys.A) ? -1 : KeyHandler.IsHeld(Keys.D) ? 1 : 0;

            foreach (IEntity entity in Entities)
            {
                // Get animation controller
                AnimationController controller = entity.GetComponent<AnimationController>();
                Physics physics = entity.GetComponent<Physics>();

                // Idle / Walk animation
                if (StateManager.IsGrounded)
                {
                    controller.SetAnimation(0);
                    ChangeAnimation(leftRightAxis, 1, 2, controller);
                }

                // Jump / Fall animation
                if (!StateManager.IsGrounded && !StateManager.IsClimbing)
                {
                    if (physics.Velocity.Y > 0)
                    {
                        controller.SetAnimation(4);
                        ChangeAnimation(leftRightAxis, 3, 4, controller);
                    }
                    else
                    {
                        controller.SetAnimation(6);
                        ChangeAnimation(leftRightAxis, 5, 6, controller);
                    }
                }

                // Climbing animation
                if (StateManager.IsClimbing)
                {
                    controller.SetAnimation(7);
                }
            }
        }

        private void ChangeAnimation(float axis, int a, int b, AnimationController controller)
        {
            if (axis < 0)
                controller.SetAnimation(a);
            else if (axis > 0)
                controller.SetAnimation(b);
        }
    }
}
