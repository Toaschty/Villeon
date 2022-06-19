using System;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Utils;

namespace Villeon.Generation
{
    public class ParticleBuilder
    {
        public static IEntity RandomParticle(Vector2 startingPosition, float timeToLive, float scale, Vector2 direction, float weight, float friction, bool isFading, string spritePath)
        {
            Random random = new Random();
            IEntity particleEntity = new Entity(new Transform(startingPosition, (float)random.NextDouble() * scale, 0.0f), "Particle");
            Particle particle = new Particle(timeToLive);
            particle.IsFading = isFading;
            particleEntity.AddComponent(particle);

            Physics physics = new Physics();
            physics.Weight = weight;
            physics.Friction = friction;
            physics.Velocity = new Vector2(direction.X * (float)((random.NextDouble() * 0.8) + 0.6), (float)((random.NextDouble() * 0.8) - 0.4));
            particleEntity.AddComponent(physics);

            Sprite sprite = Asset.GetSprite(spritePath, SpriteLayer.Foreground, true);
            particleEntity.AddComponent(sprite);

            return particleEntity;
        }

        public static IEntity StationaryParticle(Vector2 startingPosition, float timeToLive, float scale, bool isFading, string spritePath, float frameTime)
        {
            IEntity particleEntity = new Entity(new Transform(startingPosition, scale, 0.0f), "Particle");

            Particle particle = new Particle(timeToLive);
            particle.IsFading = isFading;
            particleEntity.AddComponent(particle);

            Sprite sprite = Asset.GetSprite("Sprites.Empty.png", SpriteLayer.Foreground, true);
            particleEntity.AddComponent(sprite);

            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile(spritePath, frameTime));
            particleEntity.AddComponent(animController);

            return particleEntity;
        }
    }
}
