using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation;

namespace Villeon.Systems.Update
{
    public class ParticleSpawnerSystem : System, IUpdateSystem
    {
        public ParticleSpawnerSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(ParticleSpawner));
        }

        public void Update(float time)
        {
            foreach (IEntity spawner in Entities)
            {
                Transform spawnerTransform = spawner.GetComponent<Transform>();
                ParticleSpawner particleSpawner = spawner.GetComponent<ParticleSpawner>();
                if (particleSpawner.CanSpawn())
                {
                    // Spawn the Particle
                    IEntity particle = ParticleBuilder.RandomParticle(spawnerTransform.Position + particleSpawner.Offset, 1f, 1f, 1f, particleSpawner.ParticleWeight, particleSpawner.ParticleFriction, true, particleSpawner.SpritePath, new Vector2(particleSpawner.VariationWidth, particleSpawner.VariationHeight), particleSpawner.Color);
                    Manager.GetInstance().AddEntity(particle);
                    particleSpawner.Reset();
                }
                else
                {
                    particleSpawner.DecreaseTimer(time);
                }
            }
        }
    }
}
