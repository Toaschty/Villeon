using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class PlayerExpSystem : System, IUpdateSystem
    {
        private static PlayerExpBar? _expBar;

        public PlayerExpSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(EnemyAI), typeof(Health))
                .IncludeOR(typeof(FlyingAI));

            _expBar = new PlayerExpBar();
        }

        public static void Init()
        {
            _expBar = new PlayerExpBar();
        }

        public void Update(float time)
        {
            // Update Bar
            _expBar!.Update();
        }
    }
}