﻿using System.Text;

namespace Villeon.Helper
{
    public static class StateManager
    {
        public static bool IsPlaying => !(InPause || InOptions || InShop || InMenu || InDialog);

        // Menu
        public static bool InPause { get; set; } = false;

        public static bool InOptions { get; set; } = false;

        public static bool InShop { get; set; } = false;

        // Player
        public static bool IsPlayerDead { get; set; } = false;

        public static bool IsGrounded { get; set; } = false;

        public static bool IsClimbing { get; set; } = false;

        public static bool InMenu { get; set; } = false;

        public static bool InDialog { get; set; } = false;
    }
}
