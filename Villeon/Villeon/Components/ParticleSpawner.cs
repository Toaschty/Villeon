using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Components
{
    public class ParticleSpawner : IComponent
    {
        public ParticleSpawner(float spawnRate, string spritePath)
        {
            SpawnRate = spawnRate;
            SpritePath = spritePath;
            Random random = new Random();
            CountdownToZero = (float)random.NextDouble();
        }

        public float CountdownToZero { get; private set; } = 1;

        public float SpawnRate { get; }

        public float VariationWidth { get; set; } = 0;

        public float VariationHeight { get; set; } = 0;

        public float ParticleWeight { get; set; } = 0;

        public float ParticleFriction { get; set; } = 0;

        public Vector2 Offset { get; set; } = Vector2.Zero;

        public Color4 Color { get; set; } = Color4.White;

        public Vector2 Direction { get; set; } = new Vector2(0, 0);

        public string SpritePath { get; set; }

        public bool CanSpawn()
        {
            return CountdownToZero <= 0 ? true : false;
        }

        public void Reset()
        {
            CountdownToZero = 1f;
        }

        public void DecreaseTimer(float deltaTime)
        {
            CountdownToZero -= deltaTime * SpawnRate;
        }
    }
}
