using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    [Flags]
    public enum ComponentFlag : ulong
    {
        TRANSFORM = 1,
        COLLIDER = 2,
        PHYSICS = 4,
        SPRITE = 8,
        TILE = 16,
        PLAYER = 32,
    }

    public enum ComponentIndex : int
    {
        TRANSFORM = 0,
        COLLIDER = 1,
        PHYSICS = 2,
        SPRITE = 3,
        TILE = 4,
        PLAYER = 5,
    }

    public class ComponentTypes
    {
        private static Dictionary<Type, ComponentFlag> _flags = new Dictionary<Type, ComponentFlag>();
        private static Dictionary<Type, ComponentIndex> _indexes = new Dictionary<Type, ComponentIndex>();

        public static int NUMBEROFCOMPONENTS { get; private set; } = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(p => typeof(IComponent).IsAssignableFrom(p) && !p.IsInterface)
            .Count();

        public static void Init()
        {
            // Add new Components to the Dictionary with corresponding Flag
            _flags.Add(typeof(Transform), ComponentFlag.TRANSFORM);
            _flags.Add(typeof(Collider), ComponentFlag.COLLIDER);
            _flags.Add(typeof(Physics), ComponentFlag.PHYSICS);
            _flags.Add(typeof(Sprite), ComponentFlag.SPRITE);
            _flags.Add(typeof(Tile), ComponentFlag.TILE);
            _flags.Add(typeof(Player), ComponentFlag.PLAYER);

            // Add new Components to the Dictionary with corresponding Index
            _indexes.Add(typeof(Transform), ComponentIndex.TRANSFORM);
            _indexes.Add(typeof(Collider), ComponentIndex.COLLIDER);
            _indexes.Add(typeof(Physics), ComponentIndex.PHYSICS);
            _indexes.Add(typeof(Sprite), ComponentIndex.SPRITE);
            _indexes.Add(typeof(Tile), ComponentIndex.TILE);
            _indexes.Add(typeof(Player), ComponentIndex.PLAYER);
        }

        public static ComponentFlag GetFlag(IComponent component)
        {
            if (_flags.ContainsKey(component.GetType()))
                return _flags[component.GetType()];
            return ComponentFlag.TRANSFORM;
        }

        public static ComponentIndex GetIndex(IComponent component)
        {
            return _indexes[component.GetType()];
        }

        public static ComponentIndex GetIndex<T>()
            where T : class, IComponent
        {
            return _indexes[typeof(T)];
        }

    }
}
