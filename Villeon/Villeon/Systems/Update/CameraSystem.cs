using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Villeon.Render;

namespace Villeon.Systems.Update
{
    public class CameraSystem : System, IUpdateSystem
    {
        public CameraSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(DynamicCollider), typeof(Player));
        }

        public void Update(float time)
        {
            foreach (var entity in Entities)
            {
                Transform transform = entity.GetComponent<Transform>();
                Camera.SetTracker(transform.Position + transform.Scale);
            }
        }
    }
}
