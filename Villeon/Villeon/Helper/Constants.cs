using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

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
        public static int MAX_BATCH_SIZE { get; } = 512;

        public static float SCREEN_SCALE { get; } = 9f;

        // Spawnpoints
        public static Vector2 VILLAGE_SPAWN_POINT { get; } = new Vector2(94.5f, 57f);

        public static Vector2 DUNGEON_SPAWN_POINT { get; } = new Vector2(5, 5);

        public static Vector2 SMITH_SPAWN_POINT { get; } = new Vector2(10.5f, 2f);

        public static Vector2 SHOP_SPAWN_POINT { get; } = new Vector2(16.5f, 2f);

        public static Vector2 TO_SMITH_PORTAL_POINT { get; } = new Vector2(60f, 105.5f);

        public static Vector2 FROM_SMITH_PORTAL_POINT { get; } = new Vector2(9.5f, 0f);

        public static Vector2 TO_SHOP_PORTAL_POINT { get; } = new Vector2(110f, 34.5f);

        public static Vector2 FROM_SHOP_PORTAL_POINT { get; } = new Vector2(15.5f, 0f);

        // Player
        public static int PLAYER_MAX_HEALTH { get; } = 200;
    }
}
