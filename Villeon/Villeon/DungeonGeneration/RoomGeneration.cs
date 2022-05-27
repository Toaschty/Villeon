using System;
using Newtonsoft.Json;
using Villeon.Helper;

namespace Villeon.DungeonGeneration
{
    internal class RoomGeneration
    {
        public RoomGeneration(int startRoomX, int startRoomY, int endRoomX, int endRoomY, RoomModel[,] roomModels)
        {
            Random random = new Random();
            dynamic roomJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("TileMap.RoomLayouts.json")) !;

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

                        switch (n)
                        {
                            //Layout type 1 exit left right
                            case 0:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.startRoom.zero.ToObject<string[,]>();
                                    break;
                                }

                            case 1:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.startRoom.one.ToObject<string[,]>();
                                    break;
                                }

                            case 2:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.startRoom.two.ToObject<string[,]>();
                                    break;
                                }

                            case 3:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.startRoom.three.ToObject<string[,]>();
                                    break;
                                }

                            //Layout type 2 exit left right and at the bottom
                            case 4:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.startRoom.four.ToObject<string[,]>();
                                    break;
                                }

                            case 5:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.startRoom.five.ToObject<string[,]>();
                                    break;
                                }

                            case 6:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.startRoom.six.ToObject<string[,]>();
                                    break;
                                }

                            case 7:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.startRoom.seven.ToObject<string[,]>();
                                    break;
                                }
                        }
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

                        switch (n)
                        {
                            //Layout type 3 exit left right and top
                            case 0:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.endRoom.zero.ToObject<string[,]>();
                                    break;
                                }

                            case 1:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.endRoom.one.ToObject<string[,]>();
                                    break;
                                }

                            case 2:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.endRoom.two.ToObject<string[,]>();
                                    break;
                                }

                            case 3:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.endRoom.three.ToObject<string[,]>();
                                    break;
                                }

                            //Layout type 1 exit left right
                            case 4:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.endRoom.four.ToObject<string[,]>();
                                    break;
                                }

                            case 5:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.endRoom.five.ToObject<string[,]>();
                                    break;
                                }

                            case 6:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.endRoom.six.ToObject<string[,]>();
                                    break;
                                }

                            case 7:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.endRoom.seven.ToObject<string[,]>();
                                    break;
                                }
                        }
                    }

                    // side room not in the solution path
                    else if (roomModels[i, j].RoomType == 0)
                    {
                        switch (random.Next(10))
                        {
                            case 0:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.zero.zero.ToObject<string[,]>();
                                    break;
                                }

                            case 1:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.zero.one.ToObject<string[,]>();
                                    break;
                                }

                            case 2:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.zero.two.ToObject<string[,]>();
                                    break;
                                }

                            case 3:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.zero.three.ToObject<string[,]>();
                                    break;
                                }

                            case 4:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.zero.four.ToObject<string[,]>();
                                    break;
                                }

                            case 5:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.zero.five.ToObject<string[,]>();
                                    break;
                                }

                            case 6:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.zero.six.ToObject<string[,]>();
                                    break;
                                }

                            case 7:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.zero.seven.ToObject<string[,]>();
                                    break;
                                }

                            case 8:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.zero.eight.ToObject<string[,]>();
                                    break;
                                }

                            case 9:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.zero.nine.ToObject<string[,]>();
                                    break;
                                }

                            case 10:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.zero.ten.ToObject<string[,]>();
                                    break;
                                }
                        }
                    }
                    else if (roomModels[i, j].RoomType == 1)
                    {
                        switch (random.Next(10))
                        {
                            case 0:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.one.zero.ToObject<string[,]>();
                                    break;
                                }

                            case 1:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.one.one.ToObject<string[,]>();
                                    break;
                                }

                            case 2:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.one.two.ToObject<string[,]>();
                                    break;
                                }

                            case 3:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.one.three.ToObject<string[,]>();
                                    break;
                                }

                            case 4:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.one.four.ToObject<string[,]>();
                                    break;
                                }

                            case 5:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.one.five.ToObject<string[,]>();
                                    break;
                                }

                            case 6:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.one.six.ToObject<string[,]>();
                                    break;
                                }

                            case 7:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.one.seven.ToObject<string[,]>();
                                    break;
                                }

                            case 8:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.one.eight.ToObject<string[,]>();
                                    break;
                                }

                            case 9:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.one.nine.ToObject<string[,]>();
                                    break;
                                }

                            case 10:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.one.ten.ToObject<string[,]>();
                                    break;
                                }
                        }
                    }
                    else if (roomModels[i, j].RoomType == 2)
                    {
                        switch (random.Next(10))
                        {
                            case 0:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.two.zero.ToObject<string[,]>();
                                    break;
                                }

                            case 1:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.two.one.ToObject<string[,]>();
                                    break;
                                }

                            case 2:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.two.two.ToObject<string[,]>();
                                    break;
                                }

                            case 3:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.two.three.ToObject<string[,]>();
                                    break;
                                }

                            case 4:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.two.four.ToObject<string[,]>();
                                    break;
                                }

                            case 5:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.two.five.ToObject<string[,]>();
                                    break;
                                }

                            case 6:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.two.six.ToObject<string[,]>();
                                    break;
                                }

                            case 7:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.two.seven.ToObject<string[,]>();
                                    break;
                                }

                            case 8:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.two.eight.ToObject<string[,]>();
                                    break;
                                }

                            case 9:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.two.nine.ToObject<string[,]>();
                                    break;
                                }

                            case 10:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.two.ten.ToObject<string[,]>();
                                    break;
                                }
                        }
                    }
                    else if (roomModels[i, j].RoomType == 3)
                    {
                        switch (random.Next(10))
                        {
                            case 0:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.three.zero.ToObject<string[,]>();
                                    break;
                                }

                            case 1:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.three.one.ToObject<string[,]>();
                                    break;
                                }

                            case 2:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.three.two.ToObject<string[,]>();
                                    break;
                                }

                            case 3:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.three.three.ToObject<string[,]>();
                                    break;
                                }

                            case 4:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.three.four.ToObject<string[,]>();
                                    break;
                                }

                            case 5:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.three.five.ToObject<string[,]>();
                                    break;
                                }

                            case 6:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.three.six.ToObject<string[,]>();
                                    break;
                                }

                            case 7:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.three.seven.ToObject<string[,]>();
                                    break;
                                }

                            case 8:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.three.eight.ToObject<string[,]>();
                                    break;
                                }

                            case 9:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.three.nine.ToObject<string[,]>();
                                    break;
                                }

                            case 10:
                                {
                                    roomModels[i, j].RoomLayout = roomJson.three.ten.ToObject<string[,]>();
                                    break;
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
