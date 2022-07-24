using System;
using System.Text;

namespace Villeon.Helper
{
    public static class StateManager
    {
        public static bool IsPlaying => !(InMenu || InDialog);

        public static bool InTutorial { get; set; } = false;

        public static bool InVillage { get; set; } = false;

        public static bool InDungeon { get; set; } = false;

        // Menu
        public static bool InMenu { get; set; } = false;

        // Raytracing
        public static bool RayTracingEnabled { get; set; } = false;

        // Player
        public static bool IsPlayerDead { get; set; } = false;

        public static bool IsGrounded { get; set; } = false;

        public static bool IsClimbing { get; set; } = false;

        public static bool InDialog { get; set; } = false;

        public static void SetSceneFlag(string sceneName)
        {
            // Reset all scenes
            InDungeon = false;
            InTutorial = false;
            InVillage = false;

            // Set active scene
            if (sceneName.Equals("DungeonScene"))
                InDungeon = true;
            else if (sceneName.Equals("TutorialScene"))
                InTutorial = true;
            else if (sceneName.Equals("VillageScene"))
                InVillage = true;
        }

        public static void ResetMenu()
        {
            InMenu = InDialog = false;
        }
    }
}
