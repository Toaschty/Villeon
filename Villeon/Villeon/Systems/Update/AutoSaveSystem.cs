using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.EntityManagement;
using Villeon.GUI;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.Systems.Update
{
    public class AutoSaveSystem : System, IUpdateSystem
    {
        private float _lastTimeSaved = 0;

        public AutoSaveSystem(string name)
            : base(name)
        {
        }

        public void Update(float time)
        {
            // Calculate elapsed time since last save
            float elapsedTime = Time.ElapsedTime - _lastTimeSaved;

            if (elapsedTime > Constants.AUTOSAVE_TIME)
            {
                // Save game
                SaveLoad.Save();

                _lastTimeSaved = Time.ElapsedTime;
            }
        }
    }
}
