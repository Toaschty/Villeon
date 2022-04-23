using System;
using Villeon.Components;

namespace Villeon
{
    public class Signature
    {
        public void Add<T>() where T : class, IComponent
        {
            if (typeof(T) == typeof(Transform)) 
                signature |= TRANSFORM;

            if (typeof(T) == typeof(Physics))
                signature |= PHYSICS;

            if (typeof(T) == typeof(Collider))
                signature |= COLLIDER;

            if (typeof(T) == typeof(SpriteDrawable))
                signature |= SPRITEDRAWABLE;

            if (typeof(T) == typeof(Tile))
                signature |= TILE;
        }

        public void Add(IComponent component)
        {
            if (component is Collider)
                signature |= COLLIDER;

            if (component is Transform)
                signature |= TRANSFORM;

            if (component is Physics)
                signature |= PHYSICS;

            if (component is SpriteDrawable)
                signature |= SPRITEDRAWABLE;

            if (component is Tile)
                signature |= TILE;
        }

        public bool Contains(Signature sigB)
        {
            if ((this.signature & sigB.signature) == sigB.signature)
            {
                return true;
            }
            return false;
        }

        public UInt64 signature { get; private set; } = 0;

        private UInt64 TRANSFORM = 1 << 0;
        private UInt64 PHYSICS = 1 << 1;
        private UInt64 COLLIDER = 1 << 2;
        private UInt64 SPRITEDRAWABLE = 1 << 3;
        private UInt64 TILE = 1 << 4;
    }
}