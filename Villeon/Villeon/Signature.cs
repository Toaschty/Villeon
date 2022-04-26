using System;
using Villeon.Components;

namespace Villeon
{
    public class Signature
    {
        private const ulong TRANSFORM = 1 << 0;
        private const ulong PHYSICS = 1 << 1;
        private const ulong COLLIDER = 1 << 2;
        private const ulong SPRITEDRAWABLE = 1 << 3;
        private const ulong TILE = 1 << 4;
        private const ulong PLAYER = 1 << 5;

        private ulong _signature = 0;

        public void Add<T>()
            where T : class, IComponent
        {
            if (typeof(T) == typeof(Transform))
                _signature |= TRANSFORM;

            if (typeof(T) == typeof(Physics))
                _signature |= PHYSICS;

            if (typeof(T) == typeof(Collider))
                _signature |= COLLIDER;

            if (typeof(T) == typeof(Sprite))
                _signature |= SPRITEDRAWABLE;

            if (typeof(T) == typeof(Tile))
                _signature |= TILE;

            if (typeof(T) == typeof(Player))
                _signature |= PLAYER;
        }

        public void Add(IComponent component)
        {
            if (component is Collider)
                _signature |= COLLIDER;

            if (component is Transform)
                _signature |= TRANSFORM;

            if (component is Physics)
                _signature |= PHYSICS;

            if (component is Sprite)
                _signature |= SPRITEDRAWABLE;

            if (component is Tile)
                _signature |= TILE;

            if (component is Player)
                _signature |= PLAYER;
        }

        public bool Contains(Signature sigB)
        {
            if ((_signature & sigB._signature) == sigB._signature)
            {
                return true;
            }

            return false;
        }
    }
}