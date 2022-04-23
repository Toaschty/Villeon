using OpenTK.Windowing.Common;
using System;

namespace Villeon.Helper
{
    public static class MouseHandler
    {
        private static int WheelValue { get; set; }

        public static void MouseWheel(MouseWheelEventArgs args)
        {
            WheelValue = (int)args.OffsetY;
        }

        public static int WheelChanged()
        {
            if (WheelValue == 1)
            {
                WheelValue = 0;
                return 1;
            }
            if (WheelValue == -1)
            {
                WheelValue = 0;
                return -1;
            }
            WheelValue = 0;
            return 0;
        }
    }
}