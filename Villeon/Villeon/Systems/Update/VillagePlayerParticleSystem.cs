using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class VillagePlayerParticleSystem : System, IUpdateSystem
    {
        private float _particleSpawnDelay = 0.1f;

        public VillagePlayerParticleSystem(string name)
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

                    if (player.IsWalking)
                    {
                        Vector2 direction = Vector2.Zero;
                        direction.X = KeyHandler.IsHeld(Keys.A) ? -1 : KeyHandler.IsHeld(Keys.D) ? 1 : 0;
                        direction.Y = KeyHandler.IsHeld(Keys.W) ? 1 : KeyHandler.IsHeld(Keys.S) ? -1 : 0;
                        IEntity particleEntity = ParticleBuilder.RandomParticle(transform.Position, 1f, 0.35f, direction, 0.0f, 0.3f, true, "Sprites.Dust.png");
                        Manager.GetInstance().AddEntity(particleEntity);
                    }

                    _particleSpawnDelay = 0.05f;
                }
            }
        }
    }
}
