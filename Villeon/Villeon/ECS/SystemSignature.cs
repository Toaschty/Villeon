using System;
using System.Collections.Generic;

namespace Villeon.ECS
{
    public class SystemSignature
    {
        private List<ulong> _signatures = new List<ulong>();

        public SystemSignature IncludeAND(params Type[] component)
        {
            ulong baseSignature = 0;
            for (int i = 0; i < component.Length; i++)
            {
                baseSignature |= TypeRegistry.GetFlag(component[i]);
            }

            _signatures.Add(baseSignature);

            return this;
        }

        public SystemSignature IncludeOR(params Type[] component)
        {
            for (int i = 0; i < component.Length; i++)
            {
                _signatures.Add(TypeRegistry.GetFlag(component[i]));
            }

            return this;
        }

        public void Complete()
        {
        }

        public void RemoveFromSignature<T>()
            where T : class, IComponent
        {
            for (int i = 0; i < _signatures.Count; i++)
            {
                _signatures[i] &= ~TypeRegistry.GetFlag(typeof(T));
            }
        }

        public bool Contains(ulong entitySignature)
        {
            foreach (ulong systemSignature in _signatures)
            {
                if ((entitySignature & systemSignature) == systemSignature)
                    return true;
            }

            return false;
        }
    }
}
