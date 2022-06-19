using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public enum SpriteLayer : int
    {
        ScreenGuiOnTopOfForeground,
        ScreenGuiForeground,
        ScreenGuiMiddleground,
        ScreenGuiBackground,
        ScreenGuiBehindBackground,
        ScreenGuiOverlayForeGround,
        ScreenGuiOverlayMiddleGround,
        ScreenGuiOverlayBackground,
        Collider,
        GUIForeground,
        GUIMiddleground,
        GUIBackground,
        Foreground,
        Middleground,
        Background,
    }
}
