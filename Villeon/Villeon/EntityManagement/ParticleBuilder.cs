using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.GUI;
using Villeon.Utils;

namespace Villeon.EntityManagement
{
    public class ParticleBuilder
    {
        public static IEntity RandomParticle(Vector2 startingPosition, Vector2 offset, float timeToLive, float scale, Vector2 direction, float weight, float friction, bool isFading, string spritePath, Color4 color)
        {
            Random random = new Random();
            IEntity particleEntity = new Entity(new Transform(startingPosition + offset, (float)random.NextDouble() * scale, 0.0f), "DustParticle");
            Particle particle = new Particle(timeToLive);
            particle.IsFading = isFading;
            particle.HasWind = false;
            particleEntity.AddComponent(particle);

            Physics physics = new Physics();
            physics.Weight = weight;
            physics.Friction = friction;
            physics.Velocity = new Vector2(direction.X * (float)(random.NextDouble() + 1f), direction.Y * (float)(random.NextDouble() + 1f));
            particleEntity.AddComponent(physics);

            Sprite sprite = Asset.GetSprite(spritePath, SpriteLayer.Middleground, true);
            sprite.Color = color;
            particleEntity.AddComponent(sprite);

            return particleEntity;
        }

        public static IEntity RandomParticle(Vector2 startingPosition, Vector2 offset, float timeToLive, float scale, float spreadingSpeed, float weight, float friction, bool isFading, string spritePath, Color4 color)
        {
            // Random Spreading Direction
            Random random = new Random();
            Vector2 spreadingDirection = new Vector2(((float)random.NextDouble() * spreadingSpeed) - (spreadingSpeed / 2), ((float)random.NextDouble() * spreadingSpeed) - (spreadingSpeed / 2));
            return RandomParticle(startingPosition, offset, timeToLive, scale, spreadingDirection, weight, friction, isFading, spritePath, color);
        }

        public static IEntity RandomParticle(Vector2 startingPosition, Vector2 offset, float timeToLive, float scale, float spreadingSpeed, float weight, float friction, bool isFading, string spritePath, Vector2 variationRange, Color4 color)
        {
            Random random = new Random();
            Vector2 variation = new Vector2(((float)random.NextDouble() * variationRange.X) - (variationRange.X / 2f), ((float)random.NextDouble() * variationRange.Y) - (variationRange.Y / 2f));
            IEntity particle = RandomParticle(startingPosition + variation, offset, timeToLive, scale, spreadingSpeed, weight, friction, isFading, spritePath, color);
            return particle;
        }

        public static List<IEntity> RandomParticles(Vector2 startingPosition, Vector2 offset, float timeToLive, float scale, Vector2 direction, float weight, float friction, bool isFading, string spritePath, int amount, Vector2 variationRange, Color4 color)
        {
            List<IEntity> particles = new List<IEntity>(amount);
            Random random = new Random();
            for (int i = 0; i < amount; i++)
            {
                Vector2 variation = new Vector2(((float)random.NextDouble() * variationRange.X) - (variationRange.X / 2f), ((float)random.NextDouble() * variationRange.Y) - (variationRange.Y / 2f));
                particles.Add(RandomParticle(startingPosition + variation, offset, timeToLive, scale, direction, weight, friction, isFading, spritePath, color));
            }

            return particles;
        }

        public static List<IEntity> RandomParticles(Vector2 startingPosition, Vector2 offset, float timeToLive, float scale, float spreadingSpeed, float weight, float friction, bool isFading, string spritePath, int amount, Vector2 variationRange, Color4 color)
        {
            List<IEntity> particles = new List<IEntity>(amount);
            Random random = new Random();
            for (int i = 0; i < amount; i++)
            {
                Vector2 variation = new Vector2(((float)random.NextDouble() * variationRange.X) - (variationRange.X / 2f), ((float)random.NextDouble() * variationRange.Y) - (variationRange.Y / 2f));
                particles.Add(RandomParticle(startingPosition + variation, offset, timeToLive, scale, spreadingSpeed, weight, friction, isFading, spritePath, color));
            }

            return particles;
        }

        public static IEntity AnimatedStationaryParticle(Vector2 startingPosition, float timeToLive, float scale, bool isFading, string spritePath, float frameTime, SpriteLayer spriteLayer)
        {
            IEntity particleEntity = new Entity(new Transform(startingPosition, scale, 0.0f), "AnimatedParticle");

            Particle particle = new Particle(timeToLive);
            particle.IsFading = isFading;
            particleEntity.AddComponent(particle);

            Sprite sprite = Asset.GetSprite("Sprites.Empty.png", spriteLayer, true);
            particleEntity.AddComponent(sprite);

            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile(spritePath, frameTime));
            particleEntity.AddComponent(animController);

            return particleEntity;
        }

        public static IEntity StationaryParticle(Vector2 startingPosition, float timeToLive, float scale, bool isFading, string spritePath, SpriteLayer spriteLayer)
        {
            IEntity particleEntity = new Entity(new Transform(startingPosition, scale, 0.0f), "AnimatedParticle");

            Particle particle = new Particle(timeToLive);
            particle.IsFading = isFading;
            particleEntity.AddComponent(particle);

            Sprite sprite = Asset.GetSprite(spritePath, spriteLayer, true);
            particleEntity.AddComponent(sprite);

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
                sprite.RenderLayer = SpriteLayer.Middleground;
                sprite.IsDynamic = true;
                sprite.Color = Color4.LightPink;

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

        public static List<IEntity> DashCooldownParticles(Vector2 position, float width, float height, int amount, float distance)
        {
            List<IEntity> particles = new List<IEntity>(amount);
            position += new Vector2(width / 2, height / 2);

            for (int i = 0; i < amount; i++)
            {
                float radian = i * (360f / (float)amount) * (MathF.PI / 180f);
                Vector2 direction = new Vector2(MathF.Cos(radian) * (width + distance), MathF.Sin(radian) * (height + distance));

                IEntity particleEntity = new Entity(new Transform(position + direction, 0.03f, 0f), "DashCooldownParticle");

                Particle particle = new Particle(0.2f);
                particle.TrackingPlayer = true;
                particleEntity.AddComponent(particle);

                Sprite sprite = Asset.GetSprite("Sprites.Particles.Sparkles.png", SpriteLayer.Middleground, true);
                sprite.Color = Color4.White;
                particleEntity.AddComponent(sprite);

                Physics physics = new Physics();
                physics.Weight = 0.0f;
                physics.Friction = 0.0f;
                physics.Velocity = -5 * direction;
                particleEntity.AddComponent(physics);

                particles.Add(particleEntity);
            }

            return particles;
        }
    }
}
