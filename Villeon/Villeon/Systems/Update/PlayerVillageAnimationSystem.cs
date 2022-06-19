using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class PlayerVillageAnimationSystem : System, IUpdateSystem
    {
        public PlayerVillageAnimationSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(AnimationController), typeof(Sprite), typeof(Player));
        }

        public void Update(float time)
        {
            // Get current walking direction
            int leftRightAxis = KeyHandler.IsHeld(Keys.A) ? -1 : KeyHandler.IsHeld(Keys.D) ? 1 : 0;
            int topDownAxis = KeyHandler.IsHeld(Keys.S) ? -1 : KeyHandler.IsHeld(Keys.W) ? 1 : 0;

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
