using System;
using System.Collections.Generic;
using Villeon.Components;

namespace Villeon
{
    public interface IEntity
    {
        bool Enabled { get; set; }

        string? Name { get; }

        Signature Signature { get; } // Not in runtime.. for now

        void AddComponent(IComponent component);

        public T GetComponent<T>()
            where T : class, IComponent;
    }
}