using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.EntityManagement
{
    public class Entity : IEntity
    {
        private TypeRegistry _components = new TypeRegistry();

        public Entity(Transform transform, string name)
        {
            Name = name;
            AddComponent(transform);
        }

        public Entity(string name)
        {
            Name = name;
            AddComponent(new Transform(new Vector2(0, 0), 1f, 0f));
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
            Signature = Signature.RemoveFromSignature<T>();
        }

        public T GetComponent<T>()
            where T : class, IComponent
        {
            return _components.GetInstance<T>() !;
        }

        public bool HasComponent<T>()
            where T : class, IComponent
        {
            return _components.Contains<T>();
        }
    }
}
