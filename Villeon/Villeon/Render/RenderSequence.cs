using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Render
{
    public enum RenderSequence
    {
        SpriteRenderer,
        GUIRenderer,
        ColliderRenderer,
    }

    public enum SpriteLayer
    {
        Background,
        Middleground,
        Foreground,
        //GUIBackground,
        //GUIMiddleground,
        //GUIForeground,
        Collider,
    }
}
