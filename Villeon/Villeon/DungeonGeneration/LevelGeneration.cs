using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.DungeonGeneration
{
    internal class LevelGeneration
    {
        public int StartRoomX { get; set; } = 0;

        public int EndRoomX { get; set; } = 0;

        public int StartRoomY { get; set; } = 0;

        public int EndRoomY { get; set; } = 0;

        public RoomModel[,] RoomModels { get; set; } = new RoomModel[4, 4];

        /*
        0 = not on the Paht
        1 = room with left and right exit
        2 = room with left,right and bottom exit
        3 = room with left,right and top exit
        levels consist of 4*4 amout of rooms
        */

        public void GenSolutionPath()
        {
            Random rnd = new Random();
            int rndN;

            for (int i = 0; i < RoomModels.GetLength(0); i++)
            {
                for (int j = 0; j < RoomModels.GetLength(1); j++)
                {
                    RoomModels[i, j] = new RoomModel(10, 10, 0, false, false);
                }
            }

            bool d = false;
            int roomX = StartRoomX;
            int roomY = StartRoomY;
            int lastroomX = StartRoomX;
            int lastroomY = StartRoomY;

            RoomModels[roomX, roomY].RoomType = 1;
            RoomModels[roomX, roomY].RoomIdX = roomX;
            RoomModels[roomX, roomY].RoomIdY = roomY;
            RoomModels[roomX, roomY].IsStart = true;

            while (roomY < RoomModels.GetLength(0))
            {
                d = false;
                if (roomX == 0) rndN = rnd.Next(3, 5); //right or down
                else if (roomX == 3) rndN = rnd.Next(5, 7); //left or down
                else rndN = rnd.Next(1, 5);

                // move left
                if (rndN < 3 || rndN > 5)
                {
                    if (roomX > 0)
                    {
                        if (RoomModels[roomY, roomX - 1].RoomType == 0) roomX--;
                        else rndN = 5;
                    }
                    else if (roomX < 3)
                    {
                        if (RoomModels[roomY, roomX + 1].RoomType == 0) roomX++;
                    }
                    else
                    {
                        rndN = 5;
                    }
                }

                // move right
                else if (rndN == 3 || rndN == 4)
                {
                    if (roomX < 3)
                    {
                        if (RoomModels[roomY, roomX + 1].RoomType == 0) roomX++;
                        else rndN = 5;
                    }
                    else if (roomX > 0)
                    {
                        if (RoomModels[roomY, roomX - 1].RoomType == 0) roomX--;
                    }
                    else
                    {
                        rndN = 5;
                    }
                }

                // move down
                if (rndN == 5)
                {
                    roomY++;
                    if (roomY < RoomModels.GetLength(0))
                    {
                        RoomModels[lastroomY, lastroomX].RoomType = 2;
                        RoomModels[lastroomY, lastroomX].RoomIdX = lastroomX;
                        RoomModels[lastroomY, lastroomX].RoomIdY = lastroomY;

                        RoomModels[roomY, roomX].RoomType = 3;
                        RoomModels[roomY, roomX].RoomIdX = roomX;
                        RoomModels[roomY, roomX].RoomIdY = roomY;

                        d = true;
                    }
                    else
                    {
                        EndRoomX = roomX;
                        EndRoomY = roomY - 1;
                    }
                }

                if (!d && roomY < RoomModels.GetLength(0))
                {
                    RoomModels[roomY, roomX].RoomType = 1;
                    RoomModels[roomY, roomX].RoomIdX = roomX;
                    RoomModels[roomY, roomX].RoomIdY = roomY;
                }

                lastroomX = roomX;
                lastroomY = roomY;
            }

            RoomModels[roomY - 1, roomX].IsEnd = true;
        }
    }
}
