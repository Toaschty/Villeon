﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.Helper;

namespace Villeon
{
    public class Entity : IEntity
    {
        private TypeRegistry _components = new TypeRegistry();

        public Entity(string name)
        {
            Name = name;
            AddComponent(new Transform(new Vector2(0f, 0f), 1f, 0f));
        }

        public ulong Signature { get; set; } = 0;

        public bool Enabled { get; set; } = true;

        public string Name { get; }

        public void AddComponent(IComponent component)
        {
            _components.RegisterTypeInstance(component);
            Signature = Signature.AddToSignature(component.GetType());
        }

        public void RemoveComponent<T>()
            where T : class, IComponent
        {
            _components.UnregisterTypeInstance<T>();
            Signature.RemoveFromSignature<T>();
        }

        public T GetComponent<T>()
            where T : class, IComponent
        {
            return _components.GetInstance<T>();
        }
    }
}
