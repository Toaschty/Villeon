﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Helper
{
    public static class Constants
    {
        // Dungeon
        public static int Border { get; } = 10;

        public static int MAXNPCUNLOCKS { get; } = 3;

        // Physics
        public static float GRAVITY { get; } = 40;

        public static float FRICTION { get; } = 7.5f;

        public static float JUMPSTRENGTH { get; } = 15f;

        // Movement
        public static float MOVEMENTSPEED { get; } = 75f;

        public static float DASH_POWER { get; } = 50f;

        public static float TOPDOWNMOVEMENTSPEED { get; } = 10;

        // Debug
        public static float DEBUGTIME { get; set; } = 0.00833f;

        // Rendering
        public static int MAX_BATCH_SIZE { get; } = 1024;

        public static float SCREEN_SCALE { get; } = 9f;

        // Spawnpoints
        public static Vector2 TUTORIAL_SPAWN_POINT { get; } = new Vector2(23.5f, 23.5f);

        public static Vector2 VILLAGE_SPAWN_POINT { get; } = new Vector2(143, 69);

        public static Vector2 DUNGEON_SPAWN_POINT { get; } = new Vector2(9.5f, 55f) + new Vector2(Border);

        public static Vector2 SMITH_SPAWN_POINT { get; } = new Vector2(10.5f, 2f);

        public static Vector2 SHOP_SPAWN_POINT { get; } = new Vector2(16.5f, 2f);

        public static Vector2 TO_SMITH_PORTAL_POINT { get; } = new Vector2(60f, 105.5f);

        public static Vector2 FROM_SMITH_PORTAL_POINT { get; } = new Vector2(9.5f, 0f);

        public static Vector2 TO_SHOP_PORTAL_POINT { get; } = new Vector2(110f, 34.5f);

        public static Vector2 FROM_SHOP_PORTAL_POINT { get; } = new Vector2(15.5f, 0f);

        public static Vector2 BOSS_ROOM_SPAWN_POINT { get; } = new Vector2(26f, 70f);

        // Player
        public static int PLAYER_MAX_HEALTH { get; } = 200;

        // Inventory
        public static int INVENTORY_SLOTS { get; } = 40;

        // Auto save
        public static float AUTOSAVE_TIME { get; } = 60f;
    }
}
