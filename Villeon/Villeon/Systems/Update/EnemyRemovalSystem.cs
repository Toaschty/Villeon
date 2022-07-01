using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class EnemyRemovalSystem : System, IUpdateSystem
    {
        public EnemyRemovalSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Health), typeof(EnemyAI), typeof(Health), typeof(Exp));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                Health health = entity.GetComponent<Health>();

                if (health.CurrentHealth <= 0)
                {
                    // Add experience to player
                    Exp exp = entity.GetComponent<Exp>();
                    Stats.GetInstance().GainExperience(exp.Experience);

                    Manager.GetInstance().RemoveEntity(entity);
                }
            }
        }
    }
}
