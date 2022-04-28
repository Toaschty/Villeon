using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.Helper;

namespace Villeon.Systems
{
    public class MouseClickSystem : System, IUpdateSystem
    {
        private EntitySpawner _entitySpawner = new EntitySpawner();

        public MouseClickSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Collider));
        }

        public void Update(float time)
        {
            foreach (MouseHandler.ClickedMouseButton button in MouseHandler.ClickedMouseButtons)
            {
                if (button.Button == MouseButton.Left)
                {
                    //_entitySpawner.Spawn(button.MousePosition);
                }

                if (button.Button == MouseButton.Left)
                {
                    for (int i = 0; i < Entities.Count(); i++)
                    {
                        Collider collider = Entities[i].GetComponent<Collider>();
                        if ((button.MousePosition.X > collider.Position.X) && (button.MousePosition.X < collider.Position.X + collider.Width) &&
                            (button.MousePosition.Y > collider.Position.Y) && (button.MousePosition.Y < collider.Position.Y + collider.Height))
                        {
                            //Manager.GetInstance().RemoveEntity(Entities[i]);
                            Manager.GetInstance().AddComponent(Entities[i], new Physics());
                        }
                    }
                }
            }
        }
    }
}