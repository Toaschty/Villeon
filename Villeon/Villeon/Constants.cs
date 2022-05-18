﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon
{
    public static class Constants
    {
        public static float GRAVITY { get; } = 40f;

        public static float FRICTION { get; } = 7.5f;

        public static float JUMPSTRENGTH { get; } = 15f;

        // Movement
        public static float MOVEMENTSPEED { get; } = 75f;

        public static float TOPDOWNMOVEMENTSPEED { get; } = 10;

        // Debug
        public static float DEBUGTIME { get; set; } = 0.00833f;
    }
}
