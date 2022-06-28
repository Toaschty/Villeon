using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation;
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
                //Physics physics = entity.GetComponent<Physics>();
                Transform transform = entity.GetComponent<Transform>();
                if (_particleSpawnDelay <= 0f)
                {
                    Player player = entity.GetComponent<Player>();

                    if (player.IsWalking && StateManager.IsGrounded)
                    {
                        Vector2 direction = Vector2.Zero;
                        direction.X = player.MovingLeft ? 1 : -1;
                        direction.Y = 0;
                        IEntity particleEntity = ParticleBuilder.RandomParticle(transform.Position + new Vector2(0.5f, 0f), 0.8f, 0.7f, direction, 0.0f, 0.3f, true, "Sprites.Dust.png");
                        Manager.GetInstance().AddEntity(particleEntity);
                    }

                    _particleSpawnDelay = 0.025f;
                }
            }
        }
    }
}
