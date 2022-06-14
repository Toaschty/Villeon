using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Render;
using Villeon.Systems;

namespace Villeon.Helper
{
    public static class MouseHandler
    {
        public static Vector2 ScreenMousePosition { get; private set; }

        public static Vector2 WorldMousePosition { get; private set; }

        public static List<ClickedMouseButton> ClickedMouseButtons { get; private set; } = new List<ClickedMouseButton>();

        public static List<ClickedMouseButton> ClickedRightMouseButtons { get; private set; } = new List<ClickedMouseButton>();

        public static int ClickedMouseButtonCount { get; private set; } = 0;

        public static void MouseDown(MouseButtonEventArgs args)
        {
            ClickedMouseButtons.Add(new ClickedMouseButton { Button = args.Button, MousePosition = WorldMousePosition });

            if (args.Button == MouseButton.Right)
            {
                ClickedRightMouseButtons.Add(new ClickedMouseButton { Button = args.Button, MousePosition = WorldMousePosition });
                ClickedMouseButtonCount++;
            }
        }

        public static void MouseMove(MouseMoveEventArgs args)
        {
            // Store Screen coordinaes
            ScreenMousePosition = args.Position;

            // Convert Screen to Worldcoordinates and store it
            Matrix4 fromViewportToWorldCoords = Camera.InverseViewportMatrix * Camera.GetInverseMatrix();
            WorldMousePosition = ScreenMousePosition.Transform(fromViewportToWorldCoords);
        }

        public struct ClickedMouseButton
        {
            public MouseButton Button { get; set; }

            public Vector2 MousePosition { get; set; }
        }
    }
}