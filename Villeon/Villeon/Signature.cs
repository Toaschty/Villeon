using System;
using System.Linq;
using Villeon.Components;

namespace Villeon
{
    public class Signature
    {
        private ComponentFlag _signature = ComponentFlag.TRANSFORM;

        public void Add(ComponentFlag flag) => _signature |= flag;

        public void Add(IComponent component) => _signature |= ComponentTypes.GetFlag(component);

        public bool Contains(Signature sigB)
        {
            if (_signature.HasFlag(sigB._signature))
                return true;

            return false;
        }

        public bool Has(ComponentFlag flag)
        {
            return _signature.HasFlag(flag);
        }

        public override string? ToString()
        {
            return _signature.ToString();
        }
    }
}