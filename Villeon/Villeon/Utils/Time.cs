using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Utils
{
    public static class Time
    {
        private static float _elapsedTime = 0f;

        private static float _timeOfDay = 0f;

        public static float ElapsedTime { get => _elapsedTime; set => _elapsedTime = value; }

        public static float TimeOfDay
        {
            get => MathF.Abs(MathF.Sin(_elapsedTime));
            set => _elapsedTime = value;
        }

        public static void SetTime(float time)
        {
            _elapsedTime += time;
        }
    }
}
