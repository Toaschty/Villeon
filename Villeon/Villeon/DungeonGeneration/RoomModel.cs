using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.DungeonGeneration
{
    internal class RoomModel
    {
        public RoomModel(int roomIdX, int roomIdY, int roomType, bool isStart, bool isEnd)
        {
            RoomLayout = new string[0, 0];
            RoomIdX = roomIdX;
            RoomIdY = roomIdY;
            RoomType = roomType;
            IsStart = isStart;
            IsEnd = isEnd;
        }

        public int RoomIdX { get; set; } = 0;

        public int RoomIdY { get; set; } = 0;

        public int RoomType { get; set; } = 0;

        public bool IsStart { get; set; } = false;

        public bool IsEnd { get; set; } = false;

        public string[,] RoomLayout { get; set; }
    }
}
