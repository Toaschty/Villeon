using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;

using Villeon.Systems;

namespace Villeon.Helper
{
    public static class MouseHandler
    {
        private static int WheelValue { get; set; }

        public static Vector2 MousePosition { get; private set; }

        public static Queue<Vector2> MouseClickPosition { get; private set; } = new();

        public static Queue<MouseButton> ClickedMouseButtons { get; private set; } = new();

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

        public static void MouseDown(MouseButtonEventArgs args)
        {
            // Store standard Mouse info
            ClickedMouseButtons.Enqueue(args.Button);

            // Convert the Pixel Mouse Position to World Coordinates
            Matrix4 fromViewportToWorldCoords = Camera._inverseViewportMatrix * Camera.GetInverseMatrix();
            Vector2 worldPosition = MousePosition.Transform(fromViewportToWorldCoords);
            MouseClickPosition.Enqueue(worldPosition);
        }

        public static void MouseMove(MouseMoveEventArgs args)
        {
            MousePosition = args.Position;
        }
    }
}