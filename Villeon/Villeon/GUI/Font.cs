using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.Helper;
using Villeon.Utils;
using Zenseless.OpenTK;

namespace Villeon.GUI
{
    public class Font
    {
        private static Dictionary<string, Font> _fonts = new Dictionary<string, Font>();

        private static Texture2D _sheetTexture;
        private static Sprite[] _sprites;
        private static float _fontHeight;

        public Font(Color4 color, Texture2D fontTexture, string fontJsonPath)
        {
            // Font JSON load
            JObject json = (JObject)JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText(fontJsonPath)) !;
            dynamic fontJson = json;
            int cellWidth = fontJson.gridCellWidth;
            int cellHeight = fontJson.gridCellHeight;
            int rows = fontJson.rows;
            int cols = fontJson.cols;
            int charCount = fontJson.characters.Count;

            // Init
            _sprites = new Sprite[charCount];
            _sheetTexture = fontTexture;

            int x = 0; // Start top Left
            int y = rows; // Start top left

            int charHeight = fontJson.characterHeight + fontJson.borderTop + fontJson.borderBottom;
            _fontHeight = charHeight / 8f;
            for (int i = 0; i < charCount; i++)
            {
                int charWidth = fontJson.characters[i].width + fontJson.borderLeft + fontJson.borderRight;
                Vector2[] texCoords = new Vector2[4]
                {
                    new Vector2(x * cellWidth, (y * cellHeight) - charHeight) / _sheetTexture.Height,                // bottomLeft
                    new Vector2((x * cellWidth) + charWidth, (y * cellHeight) - charHeight) / _sheetTexture.Height,  // bottomRight
                    new Vector2(x * cellWidth, y * cellHeight) / _sheetTexture.Height,                               // topLeft
                    new Vector2((x * cellWidth) + charWidth, y * cellHeight) / _sheetTexture.Height,                 // topRight
                };

                Sprite sprite = new Sprite(_sheetTexture, SpriteLayer.ScreenGuiForeground, texCoords, charWidth, charHeight);
                _sprites[i] = sprite;

                // Go to next sprite
                x += 1;

                // Go to next row
                if (x >= cols)
                {
                    x = 0;
                    y -= 1;
                }
            }
        }

        public static float FontHeight { get => _fontHeight; }

        public static void AddFont(string fontName, Font font)
        {
            if (!_fonts.ContainsKey(fontName))
                _fonts.Add(fontName, font);
        }

        public static Sprite GetCharacter(char c)
        {
            return _sprites[c - ' '];
        }

        public static Sprite GetCharacter(char c, SpriteLayer layer)
        {
            _sprites[c - ' '].RenderLayer = layer;
            return _sprites[c - ' '];
        }

        public static Sprite GetCharacter(char c, SpriteLayer layer, bool isDynamic)
        {
            _sprites[c - ' '].RenderLayer = layer;
            _sprites[c - ' '].IsDynamic = isDynamic;
            return _sprites[c - ' '];
        }
    }
}
