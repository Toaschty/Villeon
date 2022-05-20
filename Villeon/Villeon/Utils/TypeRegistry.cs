using System;
using System.Collections.Generic;
using System.Linq;

namespace Villeon
{
    public class TypeRegistry
    {
        private static Dictionary<Type, ulong> _flags = new ();

        private readonly Dictionary<Type, object> _types = new ();

        public static void Init()
        {
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(p => typeof(IComponent).IsAssignableFrom(p) && !p.IsInterface);

            ulong i = 1;
            foreach (var type in types)
            {
                _flags[type] = i;
                i *= 2;
            }
        }

        public static ulong GetFlag(Type type)
        {
            return _flags[type];
        }

        public bool Contains<TYPE>() => _types.ContainsKey(typeof(TYPE));

        public bool Contains(Type type) => _types.ContainsKey(type);

        public void RegisterTypeInstance<TYPE>(TYPE instance)
            where TYPE : class
        {
            if (instance is null) throw new ArgumentNullException(nameof(instance));
            Type type = instance.GetType();
            _types[type] = instance;
        }

        public bool UnregisterTypeInstance<TYPE>()
            where TYPE : class => _types.Remove(typeof(TYPE));

        public TYPE? GetInstance<TYPE>()
            where TYPE : class
        {
            Type type = typeof(TYPE);
            if (_types.TryGetValue(type, out var instance))
            {
                return (TYPE)instance;
            }

            return null;
        }
    }
}
