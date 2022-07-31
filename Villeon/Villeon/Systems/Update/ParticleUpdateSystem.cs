using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Utils;

namespace Villeon.Systems.Update
{
    public class ParticleUpdateSystem : System, IUpdateSystem
    {
        public ParticleUpdateSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Particle));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            Particle particle = entity.GetComponent<Particle>();
        }

        public void Update(float time)
        {
            foreach (IEntity particleEntity in Entities)
            {
                Particle particle = particleEntity.GetComponent<Particle>() !;
                Sprite sprite = particleEntity.GetComponent<Sprite>();

                if (particle.IsFading)
                {
                    Color4 color = sprite.Color;
                    color.A = particle.TTL / particle.MaxTTL;
                    sprite.Color = color;
                }
            }
        }
    }
}
