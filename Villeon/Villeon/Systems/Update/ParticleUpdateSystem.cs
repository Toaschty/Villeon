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

        private List<IEntity> _particles = new List<IEntity>();
        private IEntity? _player = null;
        private Vector2? _lastPlayerPositon = null;
        private Vector2? _playerPosition = null;

        public ParticleUpdateSystem(string name)
            : base(name)
        {
            Signature.IncludeOR(typeof(Particle), typeof(Player));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            Particle particle = entity.GetComponent<Particle>();
            Player player = entity.GetComponent<Player>();

            if (particle is not null)
                _particles.Add(entity);
            else if (player is not null)
                _player = entity;
        }

        public override void RemoveEntity(IEntity entity)
        {
            base.RemoveEntity(entity);
            Particle particle = entity.GetComponent<Particle>();
            Player player = entity.GetComponent<Player>();

            if (particle is not null)
                _particles.Remove(entity);
            else if (player is not null)
                _player = null;
        }

        public void Update(float time)
        {
            float x = ((float)_noise.Perlin(Time.ElapsedTime, 0f, Time.ElapsedTime) * 2f) - 1f;
            float y = ((float)_noise.Perlin(0f, Time.ElapsedTime, Time.ElapsedTime) * 2f) - 1f;

            if (_player is not null)
                _playerPosition = _player.GetComponent<Transform>().Position;

            Vector2 windDirection = new Vector2(x, y);
            foreach (IEntity particleEntity in _particles)
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

                if (particle.TrackingPlayer)
                {
                    if (_player is not null)
                    {
                        Vector2? deltaPositon = _playerPosition - _lastPlayerPositon;
                        if (deltaPositon is not null)
                            particleEntity.GetComponent<Transform>().Position += (Vector2)deltaPositon;
                    }
                }
            }

            _lastPlayerPositon = _playerPosition;
        }
    }
}
