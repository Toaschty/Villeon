using System;
using System.Collections.Generic;

namespace Villeon
{
    public interface IEntity
    {
        bool Enabled { get; set; }

        string Name { get;}

        Signature Signature { get; } // Not in runtime.. for now

        void AddComponent(IComponent component);

        IEnumerable<T> GetComponents<T>() where T : class, IComponent;
    }
}