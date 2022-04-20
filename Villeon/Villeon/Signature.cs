using System;
using Villeon.Components;

namespace Villeon
{
    public class Signature
    {
        public void Add<T>() where T : class, IComponent
        {
            if (typeof(T) == typeof(Transform))
            {
                Console.WriteLine("ADDED TRANSFORM!");
                signature |= TRANSFORM;
            }

            if (typeof(T) == typeof(Physics))
            {
                Console.WriteLine("ADDED PHYSICS!");
                signature |= PHYSICS;
            }

            if (typeof(T) == typeof(Collider))
            {
                Console.WriteLine("ADDED COLLIDER!");
                signature |= COLLIDER;
            }

            if (typeof(T) == typeof(SpriteDrawable))
            {
                Console.WriteLine("ADDED SPRITEDRAWABLE!");
                signature |= SPRITEDRAWABLE;
            }
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

    }
}