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
using Zenseless.OpenTK;

namespace Villeon.GUI
{
    public class Font
    {
        private Texture2D _sheetTexture;
        private Sprite[] _sprites;
        private float _fontHeight;

        public Font(Color4 color, Texture2D fontTexture, string fontJsonPath)
        {
            // Font JSON load
            dynamic fontJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText(fontJsonPath))!;
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
                    new Vector2(x * cellWidth, y * cellHeight - charHeight) / _sheetTexture.Height,                // bottomLeft
                    new Vector2(x * cellWidth + charWidth, y * cellHeight - charHeight) / _sheetTexture.Height,  // bottomRight
                    new Vector2(x * cellWidth, y * cellHeight) / _sheetTexture.Height,                               // topLeft
                    new Vector2(x * cellWidth + charWidth, y * cellHeight) / _sheetTexture.Height,                 // topRight
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

        public Texture2D SheetTexture { get => _sheetTexture; }

        public Sprite[] Sprites { get => _sprites; }
    }
}
