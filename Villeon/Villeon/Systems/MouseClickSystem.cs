using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.Helper;

namespace Villeon.Systems
{
    public class MouseClickSystem : IUpdateSystem
    {
        public MouseClickSystem(string name)
        {
            Name = name;
            Signature.Add<Collider>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; } = new List<IEntity>();

        public Signature Signature { get; } = new();

        public void Update(double time)
        {
            Collider collider;

            if (MouseHandler.MouseClickPosition.Count() != 0)
            {
                Vector2 clickPosition = MouseHandler.MouseClickPosition.Dequeue();
                for (int i = 0; i < Entities.Count(); i++)
                {
                    collider = Entities[i].GetComponent<Collider>();
                    if ((clickPosition.X > collider.Position.X) && (clickPosition.X < collider.Position.X + collider.Width) &&
                        (clickPosition.Y > collider.Position.Y) && (clickPosition.Y < collider.Position.Y + collider.Height))
                    {
                        Console.Write(collider.Position);
                        Manager.GetInstance().RemoveEntity(Entities[i]);
                        i--;
                    }
                }
            }
        }
    }
}

