using System.Text;

namespace Villeon.ECS
{
    public static class StateManager
    {
        public static bool IsPlaying => !(InPause || InOptions || InShop || InMenu);

        // Menu
        public static bool InPause { get; set; } = false;

        public static bool InOptions { get; set; } = false;

        public static bool InShop { get; set; } = false;

        // Player
        public static bool IsPlayerDead { get; set; } = false;

        public static bool IsGrounded { get; set; } = false;

        public static bool InMenu { get; set; } = false;

        // Debug
        public static bool DEBUGPAUSEACTIVE { get; set; } = false;

        public static bool DEBUGNEXTFRAME { get; set; } = false;

        public static bool DEBUGTHISFRAMEPHYSICS { get; set; } = true;
    }
}
