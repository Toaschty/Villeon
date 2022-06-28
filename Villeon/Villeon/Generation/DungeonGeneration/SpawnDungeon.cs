using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;

namespace Villeon.Generation.DungeonGeneration
{
    public class SpawnDungeon
    {

        private static int[,]? _currentDungeon;

        public static int[,]? CurrentDungeon => _currentDungeon;

        public static int[,] CreateDungeon()
        {
            // Spawn Dungeon
            LevelGeneration lvlGen = new LevelGeneration();
            lvlGen.GenSolutionPath();
            RoomGeneration roomGen = new RoomGeneration(lvlGen.StartRoomX, lvlGen.StartRoomY, lvlGen.EndRoomX, lvlGen.EndRoomY, lvlGen.RoomModels);
            for (int i = 0; i < lvlGen.RoomModels.GetLength(0); i++)
            {
                for (int j = 0; j < lvlGen.RoomModels.GetLength(1); j++)
                {
                    Console.Write(lvlGen.RoomModels[i, j].RoomType);
                }

                Console.WriteLine();
            }

            int[,] dungeon = new int[60 + (2 * Constants.Border), 120 + (2 * Constants.Border)];

            for (int i = 0; i < lvlGen.RoomModels.GetLength(0); i++)
            {
                for (int j = 0; j < lvlGen.RoomModels.GetLength(1); j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        for (int l = 0; l < 20; l++)
                        {
                            dungeon[k + (i * 10) + Constants.Border, l + (j * 20) + Constants.Border] = roomGen.RoomModels[i, j].RoomLayout[l + (k * 20)];
                        }
                    }
                }
            }

            // border around dungeon
            for (int b = 0; b < Constants.Border; b++)
            {
                for (int x = 0; x < dungeon.GetLength(1); x++)
                {
                    dungeon[b, x] = 97;
                    dungeon[dungeon.GetLength(0) - (1 + b), x] = 97;
                }

                for (int y = 0; y < dungeon.GetLength(0); y++)
                {
                    dungeon[y, b] = 97;
                    dungeon[y, dungeon.GetLength(1) - (1 + b)] = 97;
                }
            }

            _currentDungeon = dungeon;
            return dungeon;
        }
    }
}
