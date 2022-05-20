using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Render;
using Zenseless.OpenTK;

namespace Villeon.Helper
{
    public class Assets
    {
        private static Dictionary<string, Shader> _shaders = new Dictionary<string, Shader>();
        private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, SpriteSheet> _spriteSheets = new Dictionary<string, SpriteSheet>();

        public static Shader GetShader(string shaderPath)
        {
            if (_shaders.ContainsKey(shaderPath))
            {
                return _shaders[shaderPath];
            }
            else
            {
                Shader shader = new Shader();
                shader.Load(shaderPath);
                shader.Compile();
                _shaders.Add(shaderPath, shader);
                return shader;
            }
        }

        public static Texture2D GetTexture(string texturePath)
        {
            if (_textures.ContainsKey(texturePath))
            {
                return _textures[texturePath];
            }
            else
            {
                Texture2D texture = ResourceLoader.LoadContentAsTexture2D(texturePath);
                texture.MinFilter = Zenseless.OpenTK.TextureMinFilter.Nearest;
                texture.MagFilter = Zenseless.OpenTK.TextureMagFilter.Nearest;
                _textures.Add(texturePath, texture);
                return texture;
            }
        }

        public static Sprite GetSprite(string texturePath, SpriteLayer spriteLayer, bool isDynamic)
        {
            Texture2D texture = GetTexture(texturePath);
            return new Sprite(texture, spriteLayer, isDynamic);
        }

        public static void AddSpriteSheet(string spriteSheetPath, SpriteSheet spriteSheet)
        {
            if (!_spriteSheets.ContainsKey(spriteSheetPath))
            {
                _spriteSheets.Add(spriteSheetPath, spriteSheet);
            }
        }

        public static SpriteSheet? GetSpriteSheet(string spriteSheetPath)
        {
            if (_spriteSheets.ContainsKey(spriteSheetPath))
                return _spriteSheets[spriteSheetPath];

            return null; // maybe return an empty spritesheet if it doesn't exist
        }
    }
}
