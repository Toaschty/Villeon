using System;
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
        private static List<Keys> _previousPressedKeys = new List<Keys>();
        private static List<Keys> _currentPressedKeys = new List<Keys>();

        public static void KeyUp(KeyboardKeyEventArgs args)
        {
            if (_currentPressedKeys.Count != 0)
                _currentPressedKeys.Remove(args.Key);
        }

        public static void KeyDown(KeyboardKeyEventArgs args)
        {
            if (!_currentPressedKeys.Contains(args.Key))
                _currentPressedKeys.Add(args.Key);
        }

        public static bool IsPressed(Keys key)
        {
            if (_currentPressedKeys.Contains(key) && !_previousPressedKeys.Contains(key))
                return true;

            return false;
        }

        public static bool IsHeld(Keys key)
        {
            if (_currentPressedKeys.Contains(key))
                return true;

            return false;
        }

        public static void ClearKeys()
        {
            _previousPressedKeys.Clear();
            _currentPressedKeys.Clear();
        }

        public static Keys? GetLastPressedKey()
        {
            foreach (Keys key in _currentPressedKeys)
            {
                if (IsPressed(key))
                    return key;
            }

            return null;
        }

        public static void UpdateKeys()
        {
            // Clear Previous Keys
            _previousPressedKeys.Clear();

            // Add Current Keys
            foreach (Keys key in _currentPressedKeys)
            {
                _previousPressedKeys.Add(key);
            }
        }
    }
}
