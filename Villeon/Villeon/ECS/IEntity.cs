using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.ECS
{
    public interface IEntity
    {
        public ulong Signature { get; }

        public string Name { get; }

        public void AddComponent(IComponent component);

        public void RemoveComponent<T>()
            where T : class, IComponent;

        public T GetComponent<T>()
            where T : class, IComponent;

        public bool HasComponent<T>()
            where T : class, IComponent;
    }
}
