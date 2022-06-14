using System;
using Newtonsoft.Json;
using Villeon.Assets;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.Generation.DungeonGeneration
{
    internal class RoomGeneration
    {
        public RoomGeneration(int startRoomX, int startRoomY, int endRoomX, int endRoomY, RoomModel[,] roomModels)
        {
            Random random = new Random();
            dynamic roomJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("TileMap.RoomLayouts.json")) !;
            dynamic obstaclesJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("TileMap.Obstacles.json")) !;

            string[] numberStrings = new string[11];
            numberStrings[0] = "zero";
            numberStrings[1] = "one";
            numberStrings[2] = "two";
            numberStrings[3] = "three";
            numberStrings[4] = "four";
            numberStrings[5] = "five";
            numberStrings[6] = "six";
            numberStrings[7] = "seven";
            numberStrings[8] = "eight";
            numberStrings[9] = "nine";
            numberStrings[10] = "ten";

            int n;
            for (int i = 0; i < roomModels.GetLength(0); i++)
            {
                for (int j = 0; j < roomModels.GetLength(1); j++)
                {
                    if (roomModels[i, j].IsStart)
                    {
                        if (roomModels[i, j].RoomType == 1)
                        {
                            n = random.Next(0, 3);
                        }
                        else
                        {
                            n = random.Next(4, 7);
                        }

                        roomModels[i, j].RoomLayout = roomJson.startRoom[numberStrings[n]].ToObject<string[,]>();
                    }
                    else if (roomModels[i, j].IsEnd)
                    {
                        if (roomModels[i - 1, j].RoomType == 2)
                        {
                            n = random.Next(0, 3);
                        }
                        else
                        {
                            n = random.Next(4, 7);
                        }

                        roomModels[i, j].RoomLayout = roomJson.endRoom[numberStrings[n]].ToObject<string[,]>();
                    }
                    else
                    {
                        roomModels[i, j].RoomLayout = roomJson[numberStrings[roomModels[i, j].RoomType]][numberStrings[random.Next(10)]].ToObject<string[,]>();
                    }
                }
            }

            for (int i = 0; i < roomModels.GetLength(0); i++)
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int j = 0; j < roomModels.GetLength(1); j++)
                    {
                        for (int x = 1; x < 10; x++)
                        {
                            var tile = roomModels[i, j].RoomLayout[y, x];

                            if (tile == "5" || tile == "6" || tile == "8")
                            {
                                string rnd = numberStrings[random.Next(1, 8)];

                                for (int r = 0; r < 5; r++)
                                {
                                    roomModels[i, j].RoomLayout[y, x + r] = obstaclesJson[numberStrings[int.Parse(tile)]][rnd].Obs1[r];
                                    roomModels[i, j].RoomLayout[y + 1, x + r] = obstaclesJson[numberStrings[int.Parse(tile)]][rnd].Obs2[r];
                                    roomModels[i, j].RoomLayout[y + 2, x + r] = obstaclesJson[numberStrings[int.Parse(tile)]][rnd].Obs3[r];
                                }
                            }
                        }
                    }
                }
            }

            RoomModels = roomModels;
        }

        public RoomModel[,] RoomModels { get; set; }
    }
}
