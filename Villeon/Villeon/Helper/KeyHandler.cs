﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Villeon.Helper
{
    public static class KeyHandler
    {
        private static List<Keys> _pressedKeys = new List<Keys>();

        public static void KeyUp(KeyboardKeyEventArgs args)
        {
            if (_pressedKeys.Count != 0)
                _pressedKeys.Remove(args.Key);
        }

        public static void KeyDown(KeyboardKeyEventArgs args)
        {
            if (!_pressedKeys.Contains(args.Key))
                _pressedKeys.Add(args.Key);
        }

        public static bool IsPressed(Keys key)
        {
            return _pressedKeys.Contains(key);
        }

        public static void RemoveKeyHold(Keys key)
        {
            if (_pressedKeys.Count != 0)
                _pressedKeys.Remove(key);
        }
    }
}
