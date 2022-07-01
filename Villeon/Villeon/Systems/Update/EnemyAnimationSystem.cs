using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.Systems.Update
{
    public class EnemyAnimationSystem : System, IUpdateSystem
    {
        public EnemyAnimationSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(AnimationController), typeof(Sprite), typeof(EnemyAI), typeof(Physics));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                // Get animation controller
                AnimationController controller = entity.GetComponent<AnimationController>();
                Physics physics = entity.GetComponent<Physics>();

                if (physics.Velocity.X < 0)
                    controller.SetAnimation(0);
                else
                    controller.SetAnimation(1);
            }
        }
    }
}
