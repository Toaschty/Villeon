using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.Systems.Update
{
    public class DamageColoringSystem : System, IUpdateSystem
    {
        public DamageColoringSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(EnemyAI), typeof(Sprite), typeof(Effect));
            Signature.IncludeAND(typeof(Player), typeof(Sprite), typeof(Effect));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                Sprite sprite = entity.GetComponent<Sprite>();

                if (!entity.GetComponent<Effect>().Effects.ContainsKey("TookDamage"))
                {
                    // Reset Color
                    sprite.Color = OpenTK.Mathematics.Color4.White;
                }
                else
                {
                    // Colorize Entity
                    sprite.Color = OpenTK.Mathematics.Color4.HotPink;
                }
            }
        }
    }
}
