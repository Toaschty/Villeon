using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Helper;

namespace Villeon.ECS
{
    public class SystemSignature
    {
        private ulong _baseSignature = 0;
        private List<ulong> _signatures = new List<ulong>();

        public SystemSignature IncludeAND(params Type[] component)
        {
            for (int i = 0; i < component.Length; i++)
            {
                _baseSignature |= TypeRegistry.GetFlag(component[i]);
            }

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

        public SystemSignature ANDEither(params Type[] component)
        {
            for (int i = 0; i < component.Length; i++)
            {
                ulong tmp = _baseSignature;
                tmp |= TypeRegistry.GetFlag(component[i]);
                _signatures.Add(tmp);
            }

            return this;
        }

        public void Complete()
        {
            if (_baseSignature != 0)
                _signatures.Add(_baseSignature);
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
