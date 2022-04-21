﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon
{
    public class Entity : IEntity
    {
        public Entity(string name, Signature signature)
        {
            Name = name;
            Signature = signature;
        }

        private readonly List<IComponent> _components = new();

        public bool Enabled { get; set; } = true;

        public string Name { get; }

        public Signature Signature { get; }

        public void AddComponent(IComponent component)
        {
            _components.Add(component);
        }

        public T GetComponent<T>() where T : class, IComponent
        {
            foreach (var component in _components)
            {
                if (component is T)
                {
                    return (T)component;
                }
            }
            return null;
        }
    }
}
