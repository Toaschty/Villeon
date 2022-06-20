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
    internal class SpawnDungeon
    {
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

            int[,] dungeon = new int[60, 120];

            for (int i = 0; i < lvlGen.RoomModels.GetLength(0); i++)
            {
                for (int j = 0; j < lvlGen.RoomModels.GetLength(1); j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        for (int l = 0; l < 20; l++)
                        {
                            dungeon[k + (i * 10), l + (j * 20)] = roomGen.RoomModels[i, j].RoomLayout[l + (k * 20)];
                        }
                    }
                }
            }

            return dungeon;
        }
    }
}
