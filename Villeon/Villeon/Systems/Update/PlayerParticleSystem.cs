using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class PlayerParticleSystem : System, IUpdateSystem
    {
        private float _particleSpawnDelay = 0.1f;

        public PlayerParticleSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Player));
        }

        public void Update(float time)
        {
            _particleSpawnDelay -= time;

            foreach (IEntity entity in Entities)
            {
                Physics physics = entity.GetComponent<Physics>();
                Transform transform = entity.GetComponent<Transform>();
                if (_particleSpawnDelay <= 0f)
                {
                    if (Math.Abs(physics.Velocity.X) > 0.2 && StateManager.IsGrounded)
                    {
                        Random random = new Random();
                        Transform particleTranform = new Transform(transform.Position, (float)random.NextDouble() / 2f, 0.0f);
                        Entity particle = new Entity(particleTranform, "Particle");

                        Physics particlePhysics = new Physics();
                        particlePhysics.Weight = 0.0f;
                        particlePhysics.Friction = 0.0f;
                        int direction = physics.Velocity.X < 0.0f ? 1 : -1;
                        particlePhysics.Velocity = new Vector2(1 * direction * (float)((random.NextDouble() * 0.8) + 0.6), 1 * (float)((random.NextDouble() * 0.8) - 0.4));
                        particle.AddComponent(particlePhysics);

                        Sprite sprite = Asset.GetSprite("Sprites.Dust.png", SpriteLayer.Foreground, true);
                        particle.AddComponent(sprite);

                        particle.AddComponent(new Particle(0.5f));
                        Manager.GetInstance().AddEntity(particle);
                    }

                    _particleSpawnDelay = 0.05f;
                }
            }
        }
    }
}
