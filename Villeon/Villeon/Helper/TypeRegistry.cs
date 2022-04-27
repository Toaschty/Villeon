using System;
using System.Collections.Generic;

namespace Villeon.Helper
{
    public class TypeRegistry
    {
        public bool Contains<TYPE>() => _types.ContainsKey(typeof(TYPE));

        public void RegisterTypeInstance<TYPE>(TYPE instance) where TYPE : class
        {
            if (instance is null) throw new ArgumentNullException(nameof(instance));
            var type = typeof(TYPE);
            _types[type] = instance;
        }

        public bool UnregisterTypeInstance<TYPE>() where TYPE : class => _types.Remove(typeof(TYPE));

        public TYPE? GetInstance<TYPE>() where TYPE : class
        {
            var type = typeof(TYPE);
            if (_types.TryGetValue(type, out var instance))
            {
                return (TYPE)instance;
            }

            return null;
        }

        private readonly Dictionary<Type, object> _types = new();
    }
}
