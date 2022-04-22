﻿using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Helper
{
    public static class KeyHandler
    {
        public static List<Keys> pressedKeys = new List<Keys>();

        internal static void KeyUp(KeyboardKeyEventArgs args)
        {
            if (pressedKeys.Count != 0)
            {
                //pressedKeys.Clear();
                pressedKeys.Remove(args.Key);
            }
        }

        internal static void KeyDown(KeyboardKeyEventArgs args)
        {
            if (!pressedKeys.Contains(args.Key))
                pressedKeys.Add(args.Key);
        }
    }
}
