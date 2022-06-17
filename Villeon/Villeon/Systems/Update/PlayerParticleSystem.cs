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
        public PlayerParticleSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Player));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                Physics physics = entity.GetComponent<Physics>();
                Transform transform = entity.GetComponent<Transform>();
                if (Math.Abs(physics.Velocity.X) > 0.1 && StateManager.IsGrounded)
                {
                    Transform particleTranform = new Transform(transform.Position, 0.1f, 0.0f);
                    Entity particle = new Entity(particleTranform, "Particle");

                    Physics particlePhysics = new Physics();
                    int direction = physics.Velocity.X > 0.0f ? 1 : -1;
                    particlePhysics.Velocity = new Vector2(7 * direction, 7);
                    particle.AddComponent(particlePhysics);

                    Sprite sprite = Asset.GetSprite("GUI.Items.Potion_Pink.png", SpriteLayer.Foreground, true);
                    particle.AddComponent(sprite);

                    particle.AddComponent(new Particle(0.3f));
                    particle.AddComponent(new Collider(Vector2.Zero, particleTranform, 0f, 0f));
                    Manager.GetInstance().AddEntity(particle);
                }
            }
        }
    }
}
