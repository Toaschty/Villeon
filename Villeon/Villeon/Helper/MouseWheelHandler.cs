using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Common;

namespace Villeon.Helper
{
    public class MouseWheelHandler
    {
        private static int WheelValue { get; set; }

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

        public static void MouseWheel(MouseWheelEventArgs args)
        {
            WheelValue = (int)args.OffsetY;
        }
    }
}
