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
            if (_player is not null)
                _playerPosition = _player.GetComponent<Transform>().Position;

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
