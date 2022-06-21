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
        private PerlinNoise _noise = new PerlinNoise(-1);

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
            float x = ((float)_noise.Perlin(Time.ElapsedTime, 0f, Time.ElapsedTime) * 2f) - 1f;
            float y = ((float)_noise.Perlin(0f, Time.ElapsedTime, Time.ElapsedTime) * 2f) - 1f;

            Vector2 windDirection = new Vector2(x, y);
            //Console.WriteLine(windDirection);
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

                if (particle.HasWind)
                {
                    Random random = new Random();
                    Physics physics = particleEntity.GetComponent<Physics>();
                    physics.Acceleration = windDirection * 10f;
                }
            }
        }
    }
}
