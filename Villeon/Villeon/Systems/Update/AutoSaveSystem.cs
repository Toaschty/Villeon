using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.EntityManagement;
using Villeon.Generation;
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

                // Spawn SaveParticle
                IEntity savingIcon = ParticleBuilder.AnimatedStationaryParticle(new Vector2(-0.95f, -5f), 2, 0.2f, true, "Animations.Saving.png", 0.5f, Components.SpriteLayer.ScreenGuiOnTopOfForeground);
                Manager.GetInstance().AddEntity(savingIcon);
            }
        }
    }
}
