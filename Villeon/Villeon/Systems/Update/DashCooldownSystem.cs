using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.Systems.Update
{
    public class DashCooldownSystem : System, IUpdateSystem
    {
        private bool _onCooldown = false;

        public DashCooldownSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Player), typeof(Effect));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                Effect effect = entity.GetComponent<Effect>();
                if (effect.Effects.ContainsKey("DashCooldown"))
                {
                    Console.WriteLine("Cooldown: " + effect.Effects["DashCooldown"]);

                    if (effect.Effects["DashCooldown"] > .2f)
                    {
                        _onCooldown = true;
                    }
                    else if (_onCooldown)
                    {
                        _onCooldown = false;
                        Transform transform = entity.GetComponent<Transform>();
                        DynamicCollider collider = entity.GetComponent<DynamicCollider>();

                        List<IEntity> cooldownParticles = ParticleBuilder.DashCooldownParticles(transform.Position, collider.Width, collider.Height, 30, 1);
                        Manager.GetInstance().AddEntities(cooldownParticles);

                        Console.WriteLine(cooldownParticles.Count);
                    }
                }
                else
                {
                    _onCooldown = false;
                }
            }
        }
    }
}
