using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.Systems.Update
{
    public class EffectSystem : System, IUpdateSystem
    {
        public EffectSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Effect));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                Effect effect = entity.GetComponent<Effect>() !;
                foreach (var item in effect.Effects)
                    effect.Effects[item.Key] -= time;

                var effectsToRemove = effect.Effects.Where(f => f.Value < 0).ToArray();
                foreach (var item in effectsToRemove)
                    effect.Effects.Remove(item.Key);
            }
        }
    }
}
