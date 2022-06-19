using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;
using Villeon.Utils;

namespace Villeon.Generation
{
    public class ParticleBuilder
    {
        public static IEntity RandomParticle(Vector2 startingPosition, float timeToLive, float scale, Vector2 direction, float weight, float friction, bool isFading, string spritePath)
        {
            Random random = new Random();
            IEntity particleEntity = new Entity(new Transform(startingPosition, (float)random.NextDouble() * scale, 0.0f), "DustParticle");
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
            IEntity particleEntity = new Entity(new Transform(startingPosition, scale, 0.0f), "AnimatedParticle");

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

        public static IEntity[] DamageParticles(string damage, Vector2 startingPosition, float timeToLive, float scale, bool isFading)
        {
            Random random = new Random();

            Text text = new Text(damage, startingPosition, "Alagard", 0f, 0f, scale);
            Vector2 textVelocity = new Vector2((float)(random.NextDouble() * 5f), 5f);
            foreach (IEntity entity in text.GetEntities())
            {
                Sprite sprite = entity.GetComponent<Sprite>();
                sprite.RenderLayer = SpriteLayer.Foreground;
                sprite.IsDynamic = true;

                // Add the particle
                entity.AddComponent(new Particle(timeToLive));

                // Add Physics
                Physics physics = new Physics();
                physics.Weight = 0.5f;
                physics.Friction = 0.0f;
                physics.Velocity = textVelocity;
                entity.AddComponent(physics);
            }

            return text.GetEntities();
        }
    }
}
