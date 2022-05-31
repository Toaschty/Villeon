using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Villeon.Helper
{
    public static class WindowCreator
    {
        private static GameWindow? _window;

        public static GameWindow CreateWindow()
        {
            NativeWindowSettings windowSettings = new NativeWindowSettings();
            windowSettings.Profile = ContextProfile.Compatability;
            _window = new GameWindow(GameWindowSettings.Default, windowSettings);

            MonitorInfo monitorInfo = Monitors.GetMonitorFromWindow(_window);
            _window.Size = new Vector2i(monitorInfo.HorizontalResolution, monitorInfo.VerticalResolution) / 2;
            _window.VSync = VSyncMode.Off;
            _window.UpdateFrequency = 120;
            _window.RenderFrequency = 120;
            return _window;
        }

        public static void CloseWindow()
        {
            if (_window is not null)
                _window.Close();
        }
    }
}
