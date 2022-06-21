using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.Systems.Update
{
    public class EnemyRemovalSystem : System, IUpdateSystem
    {
        public EnemyRemovalSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Health), typeof(EnemyAI), typeof(Health));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                Health health = entity.GetComponent<Health>();

                if (health.CurrentHealth <= 0)
                    Manager.GetInstance().RemoveEntity(entity);
            }
        }
    }
}
