using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.Utils;

namespace Villeon.EntityManagement
{
    public static class UlongSignatureExtensions
    {
        public static ulong AddToSignature(this ulong signature, Type component)
        {
            return signature |= TypeRegistry.GetFlag(component);
        }

        public static ulong RemoveFromSignature<T>(this ulong signature)
            where T : class, IComponent
        {
            return signature &= ~TypeRegistry.GetFlag(typeof(T)); // Pain
        }

        public static bool Contains(this ulong sig, ulong signature)
        {
            if ((sig & signature) == signature)
                return true;
            return false;
        }
    }
}
