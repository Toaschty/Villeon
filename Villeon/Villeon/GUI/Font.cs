using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        private Texture2D _sheetTexture;
        private Sprite[] _sprites;
        private float _fontHeight;

        // FONT
        private dynamic _fontJson;

        public Font(Color4 color, Texture2D fontTexture, string fontJsonPath)
        {
            // Font JSON load
            _fontJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText(fontJsonPath)) !;
            int cellWidth = _fontJson.gridCellWidth;
            int cellHeight = _fontJson.gridCellHeight;
            int rows = _fontJson.rows;
            int cols = _fontJson.cols;
            int charCount = _fontJson.characters.Count;

            // Init
            _sprites = new Sprite[charCount];
            _sheetTexture = fontTexture;

            int x = 0; // Start top Left
            int y = rows; // Start top left

            int charHeight = _fontJson.characterHeight + _fontJson.borderTop + _fontJson.borderBottom;
            _fontHeight = charHeight / 8f;
            for (int i = 0; i < charCount; i++)
            {
                int charWidth = _fontJson.characters[i].width + _fontJson.borderLeft + _fontJson.borderRight;
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

        public float FontHeight { get => _fontHeight; }

        public Sprite GetCharacter(char c)
        {
            return _sprites[c - ' '];
        }

        public Sprite GetCharacter(char c, SpriteLayer layer)
        {
            _sprites[c - ' '].RenderLayer = layer;
            return _sprites[c - ' '];
        }

        public Sprite GetCharacter(char c, SpriteLayer layer, bool isDynamic)
        {
            _sprites[c - ' '].RenderLayer = layer;
            _sprites[c - ' '].IsDynamic = isDynamic;
            return _sprites[c - ' '];
        }
    }
}
