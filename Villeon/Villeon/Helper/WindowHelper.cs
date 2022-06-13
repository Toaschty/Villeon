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
    public static class WindowHelper
    {
        public static GameWindow? GameWindow { get; private set; }

        public static GameWindow CreateWindow()
        {
            NativeWindowSettings windowSettings = new NativeWindowSettings();
            windowSettings.Profile = ContextProfile.Core;
            GameWindow = new GameWindow(GameWindowSettings.Default, windowSettings);

            MonitorInfo monitorInfo = Monitors.GetMonitorFromWindow(GameWindow);
            GameWindow.Size = new Vector2i(monitorInfo.HorizontalResolution, monitorInfo.VerticalResolution) / 2;
            GameWindow.VSync = VSyncMode.Off;
            GameWindow.UpdateFrequency = 120;
            GameWindow.RenderFrequency = 120;
            return GameWindow;
        }

        public static void CloseWindow()
        {
            if (GameWindow is not null)
                GameWindow.Close();
        }
    }
}
