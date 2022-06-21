using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;

namespace Villeon.Systems.Update
{
    public class PlayerExpSystem : System, IUpdateSystem
    {
        private PlayerExpBar _expBar;
        private Exp _playerExp;

        public PlayerExpSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(EnemyAI), typeof(Health));

            _playerExp = new Exp(100);
            _expBar = new PlayerExpBar(_playerExp);
        }

        public void Update(float time)
        {
            // Go through all the Enemies
            foreach (IEntity entity in Entities)
            {
                Health health = entity.GetComponent<Health>();

                // Go to next Enemy if the health isn't 0
                if (health.CurrentHealth > 0)
                    continue;

                // Enemy died
                _expBar!.UpdateExpbar(10);
            }
        }
    }
}