using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;

namespace Villeon.Systems.Update
{
    public class MobDropSystem : System, IUpdateSystem
    {
        public MobDropSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Health), typeof(Drops));
        }

        public void Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
