using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class EnemyAI : IComponent
    {
        public EnemyAI(int damage)
        {
            Damage = damage;
        }

        public int Damage { get; set; }
    }
}
