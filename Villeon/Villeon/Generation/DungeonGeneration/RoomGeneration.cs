using System;
using System.IO;
using System.Linq;
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
            string[] numberStrings = new string[4];
            numberStrings[0] = "Filler";
            numberStrings[1] = "Path";
            numberStrings[2] = "PathBottomOpen";
            numberStrings[3] = "PathTopOpen";
            string fileName;
            dynamic roomJson;

            for (int i = 0; i < roomModels.GetLength(0); i++)
            {
                for (int j = 0; j < roomModels.GetLength(1); j++)
                {
                    if (roomModels[i, j].IsStart)
                    {
                        if (roomModels[i, j].RoomType == 1)
                        {
                            fileName = Getrandomfile(Directory.GetCurrentDirectory() + "../../../../Assets/TileMap/DungeonRooms/Start/Pathway");

                            roomJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("TileMap.DungeonRooms.Start.Pathway." + fileName)) !;
                        }
                        else
                        {
                            fileName = Getrandomfile(Directory.GetCurrentDirectory() + "../../../../Assets/TileMap/DungeonRooms/Start/PathwayBottomOpen");
                            roomJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("TileMap.DungeonRooms.Start.PathwayBottomOpen." + fileName)) !;
                        }

                        roomModels[i, j].RoomLayout = roomJson.layers[0].data.ToObject<int[]>();
                    }
                    else if (roomModels[i, j].IsEnd)
                    {
                        if (roomModels[i - 1, j].RoomType == 2)
                        {
                            fileName = Getrandomfile(Directory.GetCurrentDirectory() + "../../../../Assets/TileMap/DungeonRooms/End/PathwayTopOpen");
                            roomJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("TileMap.DungeonRooms.End.PathwayTopOpen." + fileName)) !;
                        }
                        else
                        {
                            fileName = Getrandomfile(Directory.GetCurrentDirectory() + "../../../../Assets/TileMap/DungeonRooms/End/Pathway");
                            roomJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("TileMap.DungeonRooms.End.Pathway." + fileName)) !;
                        }

                        roomModels[i, j].RoomLayout = roomJson.layers[0].data.ToObject<int[]>();
                    }
                    else
                    {
                        if (i == 0)
                        {
                            fileName = Getrandomfile(Directory.GetCurrentDirectory() + "../../../../Assets/TileMap/DungeonRooms/" + numberStrings[roomModels[i, j].RoomType]);
                            roomJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("TileMap.DungeonRooms." + numberStrings[roomModels[i, j].RoomType] + "." + fileName)) !;
                        }
                        else if (roomModels[i - 1, j].RoomType == 2 && roomModels[i, j].RoomType == 2)
                        {
                            fileName = Getrandomfile(Directory.GetCurrentDirectory() + "../../../../Assets/TileMap/DungeonRooms/" + numberStrings[roomModels[i, j].RoomType] + "/PathTopBottomOpen");
                            roomJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("TileMap.DungeonRooms." + numberStrings[roomModels[i, j].RoomType] + ".PathTopBottomOpen." + fileName)) !;
                        }
                        else
                        {
                            fileName = Getrandomfile(Directory.GetCurrentDirectory() + "../../../../Assets/TileMap/DungeonRooms/" + numberStrings[roomModels[i, j].RoomType]);
                            roomJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("TileMap.DungeonRooms." + numberStrings[roomModels[i, j].RoomType] + "." + fileName)) !;
                        }

                        roomModels[i, j].RoomLayout = roomJson.layers[0].data.ToObject<int[]>();
                    }
                }
            }

            /*
            for (int i = 0; i < roomModels.GetLength(0); i++)
            {
                for (int y = 0; y < 10; y++)
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
            */
            RoomModels = roomModels;
        }

        public RoomModel[,] RoomModels { get; set; }

        private string Getrandomfile(string path)
        {
            string? file = " ";
            if (!string.IsNullOrEmpty(path))
            {
                var extensions = new string[] { ".tmj", ".json" };
                try
                {
                    var di = new DirectoryInfo(path);
                    var files = di.GetFiles("*.*").Where(f => extensions.Contains(f.Extension.ToLower()));
                    Random rnd = new Random();
                    file = files.ElementAt(rnd.Next(0, files.Count())).Name;
                }
                catch
                {
                }
            }

            return file;
        }
    }
}
