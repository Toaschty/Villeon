using Villeon.Components;

namespace Villeon
{
    public class Signature
    {
        public void Add<T>() where T : class, IComponent
        {
            if (typeof(T) == typeof(Transform))
            {
                Console.WriteLine("TRANSFORM!");
                signature |= TRANSFORM;
            }

            if (typeof(T) == typeof(Physics))
            {
                Console.WriteLine("PHYSICS!");
                signature |= PHYSICS;
            }

            if (typeof(T) == typeof(Collider))
            {
                Console.WriteLine("COLLIDER!");
                signature |= COLLIDER;
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

    }
}