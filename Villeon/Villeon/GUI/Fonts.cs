using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;

namespace Villeon.GUI
{
    public class Fonts
    {
        private static Dictionary<string, Font> _fonts = new Dictionary<string, Font>();

        public static void AddFont(string fontName, Font font)
        {
            if (!_fonts.ContainsKey(fontName))
                _fonts.Add(fontName, font);
        }

        public static float GetFontHeight(string fontName)
        {
            return _fonts[fontName].FontHeight;
        }

        public static Sprite GetCharacter(string fontName, char c)
        {
            return _fonts[fontName].Sprites[c - ' '];
        }

        public static Sprite GetCharacter(string fontName, char c, SpriteLayer layer)
        {
            _fonts[fontName].Sprites[c - ' '].RenderLayer = layer;
            return _fonts[fontName].Sprites[c - ' '];
        }

        public static Sprite GetCharacter(string fontName, char c, SpriteLayer layer, bool isDynamic)
        {
            _fonts[fontName].Sprites[c - ' '].RenderLayer = layer;
            _fonts[fontName].Sprites[c - ' '].IsDynamic = isDynamic;
            return _fonts[fontName].Sprites[c - ' '];
        }
    }
}
