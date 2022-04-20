using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Helper
{
    public static class WindowCreator
    {
        public static GameWindow CreateWindow()
        {
            NativeWindowSettings windowSettings = new NativeWindowSettings();
            windowSettings.Profile = ContextProfile.Compatability;
            GameWindow window = new GameWindow(GameWindowSettings.Default, windowSettings);

            MonitorInfo monitorInfo = Monitors.GetMonitorFromWindow(window);
            window.Size = new Vector2i(monitorInfo.HorizontalResolution, monitorInfo.VerticalResolution) / 2;
            window.VSync = VSyncMode.On;
            //window.UpdateFrequency = 120;
            //window.RenderFrequency = 60;
            return window;
        }
    }
}
