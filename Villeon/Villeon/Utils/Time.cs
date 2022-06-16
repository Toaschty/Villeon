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

        private static float _currentDayTime = 0f;

        public static float ElapsedTime { get => _elapsedTime; }

        public static float CurrentDayTime { get => _currentDayTime; }

        public static void SetTime(float time)
        {
            _elapsedTime += time;
            _currentDayTime += time / 6f;

            if (_currentDayTime >= 24.00f)
            {
                _currentDayTime = 0f;
            }
        }
    }
}
