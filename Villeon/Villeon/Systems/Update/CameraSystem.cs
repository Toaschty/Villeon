using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;

namespace Villeon.Systems
{
    public class CameraSystem : System, IUpdateSystem
    {
        public CameraSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Collider));
            Signature = Signature.AddToSignature(typeof(Player));
        }

        public void Update(float time)
        {
            foreach (var entity in Entities)
            {
                Transform transform = entity.GetComponent<Transform>();
                Camera.SetTracker(transform.Position);
            }
        }
    }
}
