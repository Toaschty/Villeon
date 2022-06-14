using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;

namespace Villeon.Systems.TriggerSystems
{
    internal class TriggerSystem : System, IUpdateSystem
    {
        public TriggerSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Trigger));
        }

        public void Update(float time)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Trigger trigger = Entities.ElementAt(i).GetComponent<Trigger>();

                if (trigger.ToBeRemoved)
                {
                    // Remove Trigger
                    Manager.GetInstance().RemoveEntity(Entities.ElementAt(i));
                    i--;
                }
                else
                {
                    // Set changed Trigger Position
                    Transform transform = Entities.ElementAt(i).GetComponent<Transform>();
                    trigger.Position = transform.Position + trigger.Offset;

                    // Reduce Lifetime of Trigger
                    trigger.Time -= time;
                }
            }
        }
    }
}
