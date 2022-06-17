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
                        Transform particleTranform = new Transform(transform.Position, 0.1f, 0.0f);
                        Entity particle = new Entity(particleTranform, "Particle");

                        Physics particlePhysics = new Physics();
                        int direction = physics.Velocity.X > 0.0f ? 1 : -1;
                        Random random = new Random();
                        particlePhysics.Velocity = new Vector2(4 * direction * (float)((random.NextDouble() * 0.8) + 0.6), 3 * (float)((random.NextDouble() * 0.8) + 0.6));
                        //particle.AddComponent(particlePhysics);

                        Sprite sprite = Asset.GetSprite("GUI.Items.Potion_Pink.png", SpriteLayer.Foreground, true);
                        particle.AddComponent(sprite);

                        particle.AddComponent(new Particle(0.3f));
                        Manager.GetInstance().AddEntity(particle);
                    }

                    _particleSpawnDelay = 0.1f;
                }
            }
        }
    }
}
