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
    public class ParticleSystem : System, IUpdateSystem
    {
        public ParticleSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Particle));
        }

        public void Update(float time)
        {
            List<IEntity> particlesToBeRemoved = new List<IEntity>();
            foreach (IEntity entity in Entities)
            {
                Particle particle = entity.GetComponent<Particle>() !;
                particle.TTL -= time;

                Transform transform = entity.GetComponent<Transform>();
                transform.Scale = new Vector2(0.1f * (particle.TTL / particle.MaxTTL));

                if (particle.TTL <= 0.0f)
                    particlesToBeRemoved.Add(entity);
            }

            for (int i = 0; i < particlesToBeRemoved.Count; i++)
                Manager.GetInstance().RemoveEntity(particlesToBeRemoved[0]);
        }
    }
}
