using System.Text;

namespace Villeon
{
    public static class StateManager
    {
        public static bool IsPlaying => !(InPause || InOptions || InShop);

        // Menu
        public static bool InPause { get; set; } = false;

        public static bool InOptions { get; set; } = false;

        public static bool InShop { get; set; } = false;

        // Player
        public static bool IsDead { get; set; } = false;

        public static bool IsGrounded { get; set; } = false;

        public static bool InDialog { get; set; } = false;

        public static bool InInventory { get; set; } = false;

        public static bool InEquipment { get; set; } = false;

        public static bool InMap { get; set; } = false;

        public static bool InDungeonSelection { get; set; } = false;

        // Debug
        public static bool DEBUGPAUSEACTIVE { get; set; } = false;

        public static bool DEBUGNEXTFRAME { get; set; } = false;

        public static bool DEBUGTHISFRAMEPHYSICS { get; set; } = true;
    }
}
