using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.Systems.Update
{
    public class ParticleRemovalSystem : System, IUpdateSystem
    {
        public ParticleRemovalSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Particle));
        }

        public void Update(float time)
        {
            List<IEntity> particlesToBeRemoved = new List<IEntity>();
            foreach (IEntity particleEntity in Entities)
            {
                Particle particle = particleEntity.GetComponent<Particle>() !;
                particle.TTL -= time;
                if (particle.TTL <= 0.0f)
                    particlesToBeRemoved.Add(particleEntity);
            }

            Manager.GetInstance().RemoveEntities(particlesToBeRemoved);
        }
    }
}
