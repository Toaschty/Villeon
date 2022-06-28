using System.Collections.Generic;
using OpenTK.Mathematics;
using Villeon.Components;
using Zenseless.OpenTK;

namespace Villeon.Assets
{
    public class Asset
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
                texture.MinFilter = TextureMinFilter.Nearest;
                texture.MagFilter = TextureMagFilter.Nearest;
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

        public static SpriteSheet GetSpriteSheet(string spriteSheetPath) => _spriteSheets[spriteSheetPath];

        public static void LoadRessources()
        {
            // Shaders
            GetShader("Shaders.light");

            // Font
            AddSpriteSheet("Fonts.VilleonFont.png", new SpriteSheet(Color4.White, GetTexture("Fonts.VilleonFont.png"), 8, 12, 95));
            AddSpriteSheet("Fonts.VilleonFontThin.png", new SpriteSheet(Color4.White, GetTexture("Fonts.VilleonFontThin.png"), 7, 12, 95));
            AddSpriteSheet("Fonts.HenksFont.png", new SpriteSheet(Color4.White, GetTexture("Fonts.HenksFont.png"), 5, 8, 95));

            // Player Animations
            AddSpriteSheet("Animations.player_idle.png", new SpriteSheet(Color4.White, GetTexture("Animations.player_idle.png"), 16, 34, 5, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.player_walk_up.png", new SpriteSheet(Color4.White, GetTexture("Animations.player_walk_up.png"), 16, 34, 6, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.player_walk_down.png", new SpriteSheet(Color4.White, GetTexture("Animations.player_walk_down.png"), 16, 34, 6, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.player_walk_left.png", new SpriteSheet(Color4.White, GetTexture("Animations.player_walk_left.png"), 16, 34, 6, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.player_walk_right.png", new SpriteSheet(Color4.White, GetTexture("Animations.player_walk_right.png"), 16, 34, 6, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.slash_attack_left.png", new SpriteSheet(Color4.White, GetTexture("Animations.slash_attack_left.png"), 32, 32, 4, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.slash_attack_right.png", new SpriteSheet(Color4.White, GetTexture("Animations.slash_attack_right.png"), 32, 32, 4, 0, SpriteLayer.Foreground));

            // Dungeon Player Animations
            AddSpriteSheet("Animations.d_player_idle.png", new SpriteSheet(Color4.White, GetTexture("Animations.d_player_idle.png"), 16, 32, 5, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.d_player_walk_left.png", new SpriteSheet(Color4.White, GetTexture("Animations.d_player_walk_left.png"), 16, 32, 6, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.d_player_walk_right.png", new SpriteSheet(Color4.White, GetTexture("Animations.d_player_walk_right.png"), 16, 32, 6, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.d_player_jump_left.png", new SpriteSheet(Color4.White, GetTexture("Animations.d_player_jump_left.png"), 16, 32, 2, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.d_player_jump_right.png", new SpriteSheet(Color4.White, GetTexture("Animations.d_player_jump_right.png"), 16, 32, 2, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.d_player_fall_left.png", new SpriteSheet(Color4.White, GetTexture("Animations.d_player_fall_left.png"), 16, 32, 5, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.d_player_fall_right.png", new SpriteSheet(Color4.White, GetTexture("Animations.d_player_fall_right.png"), 16, 32, 5, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.d_player_climb.png", new SpriteSheet(Color4.White, GetTexture("Animations.d_player_climb.png"), 16, 32, 6, 0, SpriteLayer.Foreground));

            // Enemy Animations
            AddSpriteSheet("Animations.slime_jumping.png", new SpriteSheet(Color4.White, GetTexture("Animations.slime_jumping.png"), 32, 39, 13, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.slime_attack_left.png", new SpriteSheet(Color4.White, GetTexture("Animations.slime_attack_left.png"), 32, 32, 4, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Animations.slime_attack_right.png", new SpriteSheet(Color4.White, GetTexture("Animations.slime_attack_right.png"), 32, 32, 4, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Enemies.Slime.slime_jumping.png", new SpriteSheet(Color4.White, GetTexture("Enemies.Slime.slime_jumping.png"), 32, 39, 13, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Enemies.Bat.bat_left.png", new SpriteSheet(Color4.White, GetTexture("Enemies.Bat.bat_left.png"), 32, 32, 4, 0, SpriteLayer.Foreground));
            AddSpriteSheet("Enemies.Bat.bat_right.png", new SpriteSheet(Color4.White, GetTexture("Enemies.Bat.bat_right.png"), 32, 32, 4, 0, SpriteLayer.Foreground));

            // Map Animations
            AddSpriteSheet("Sprites.PortalAnimation.png", new SpriteSheet(Color4.White, GetTexture("Sprites.PortalAnimation.png"), 48, 56, 17, 0, SpriteLayer.Middleground));

            // GUI
            AddSpriteSheet("Animations.InventorySlotSwapIndicator.png", new SpriteSheet(Color4.White, GetTexture("Animations.InventorySlotSwapIndicator.png"), 32, 32, 6, 0, SpriteLayer.ScreenGuiForeground));

            // TileMap
            AddSpriteSheet("TileMap.TilesetImages.DungeonCrumblyCave.png", new SpriteSheet(Color4.White, GetTexture("TileMap.TilesetImages.DungeonCrumblyCave.png"), 8, 8, 64, 0, SpriteLayer.Foreground));
            AddSpriteSheet("TileMap.TilesetImages.DungeonHellishHole.png", new SpriteSheet(Color4.White, GetTexture("TileMap.TilesetImages.DungeonHellishHole.png"), 8, 8, 64, 0, SpriteLayer.Foreground));
        }
    }
}
