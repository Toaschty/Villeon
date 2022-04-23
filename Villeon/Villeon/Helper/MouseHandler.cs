using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;

using Villeon.Systems;

namespace Villeon.Helper
{
    public static class MouseHandler
    {
        private static int WheelValue { get; set; }

        public static Vector2 MousePosition { get; private set; }

        public static Stack<Vector2> MouseClicks { get; private set; } = new Stack<Vector2>();

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

        internal static void MouseDown(MouseButtonEventArgs args)
        {
            Matrix4 fromViewportToWorldCoords = Camera._inverseViewportMatrix * Camera.GetInverseMatrix();
            Vector2 worldPosition = MousePosition.Transform(fromViewportToWorldCoords);

            MouseClicks.Push(worldPosition);
        }

        internal static void MouseMove(MouseMoveEventArgs args)
        {
            MousePosition = args.Position;
        }
    }
}