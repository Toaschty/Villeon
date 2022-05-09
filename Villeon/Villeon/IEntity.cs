using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon
{
    public interface IEntity
    {
        public ulong Signature { get; }

        public bool Enabled { get; set; }

        public string Name { get; }

        public void AddComponent(IComponent component);

        public void RemoveComponent<T>()
            where T : class, IComponent;

        public T GetComponent<T>()
            where T : class, IComponent;
    }
}
