using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;

namespace Villeon.Systems
{
    public class CameraSystem : IUpdateSystem
    {
        public CameraSystem(string name)
        {
            Name = name;
            Signature.Add<Player>();
            Signature.Add<Collider>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; private set; } = new();

        public Signature Signature { get; private set; } = new();

        public void Update(float time)
        {
            foreach (var entity in Entities)
            {
                Collider collider = entity.GetComponent<Collider>();
                Camera.SetTracker(collider.Center);
            }
        }
    }
}
