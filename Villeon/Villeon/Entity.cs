using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;

namespace Villeon
{
    public class Entity : IEntity
    {
        private IComponent[] _components = new IComponent[ComponentTypes.NUMBEROFCOMPONENTS];

        public Entity(string name)
        {
            Name = name;
            _components[(int)ComponentIndex.TRANSFORM] = new Transform(new Vector2(0f, 0f), 1f, 0f);
        }

        public bool Enabled { get; set; } = true;

        public string Name { get; }

        public Signature Signature { get; } = new ();

        public void AddComponent(IComponent component)
        {
            // Add the component if the component flag isn't already set
            if (!Signature.Has(ComponentTypes.GetFlag(component)))
            {
                _components[(int)ComponentTypes.GetIndex(component)] = component;
                Signature.Add(component);
            }
        }

        public T GetComponent<T>()
            where T : class, IComponent
        {
            return _components[(int)ComponentTypes.GetIndex<T>()] as T;
        }
    }
}
