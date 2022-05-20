using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon
{
    public static class Constants
    {
        public static float GRAVITY { get; } = 40;

        public static float FRICTION { get; } = 7.5f;

        public static float JUMPSTRENGTH { get; } = 15f;

        // Movement
        public static float MOVEMENTSPEED { get; } = 75f;

        public static float TOPDOWNMOVEMENTSPEED { get; } = 10;

        // Debug
        public static float DEBUGTIME { get; set; } = 0.00833f;

        // Rendering
        public static int MAX_BATCH_SIZE { get; } = 1024;

        // Spawnpoints
        public static Vector2 VILLAGE_SPAWN_POINT { get; } = new Vector2(32, 32);

        public static Vector2 DUNGEON_SPAWN_POINT { get; } = new Vector2(5, 5);
    }
}
