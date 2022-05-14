using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;
using Villeon.Render;
using Villeon.Systems;
using Villeon.Systems.Update;
using Zenseless.OpenTK;
using Zenseless.Resources;

namespace Villeon
{
    public class Game
    {
        private Scene _dungeonScene = new ("DungeonScene");
        private Scene _villageScene = new ("VillageScene");
        private Matrix4 _refCameraMatrix = Matrix4.Identity;
        private IEntity _entity;
        private Renderer _renderer;
        private FPS? _fps;

        public void Start()
        {
            GameWindow gameWindow = WindowCreator.CreateWindow();
            _fps = new FPS(gameWindow);
            Init();
            SceneLoader.AddScene(_dungeonScene);
            SceneLoader.AddScene(_villageScene);

            // Load scene
            SceneLoader.LoadScene("DungeonScene");
            //SceneLoader.LoadScene("VillageScene");

            gameWindow.KeyDown += KeyHandler.KeyDown;
            gameWindow.KeyUp += KeyHandler.KeyUp;
            gameWindow.MouseWheel += MouseHandler.MouseWheel;
            gameWindow.MouseDown += MouseHandler.MouseDown;
            gameWindow.MouseMove += MouseHandler.MouseMove;
            gameWindow.UpdateFrame += UpdateFrame;
            gameWindow.RenderFrame += RenderFrame;
            gameWindow.RenderFrame += _ => gameWindow.SwapBuffers();
            gameWindow.Resize += Resize;
            gameWindow.Run();
        }

        private void Resize(ResizeEventArgs args)
        {
            Camera.Resize(args.Width, args.Height);
            GL.Viewport(0, 0, args.Width, args.Height);
        }

        private void Init()
        {
            TypeRegistry.Init();
            LoadResources();

            // Platformer Scene
            TileMap tileMap = new TileMap("DungeonTileMap.tmx", true);
            _entity = new Entity(new Transform(new Vector2(-0.5f, 0.5f), new Vector2(1.0f, 1.0f), 0f), "Marin");
            _entity.AddComponent(new Physics());
            _entity.AddComponent(new Collider(new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), 1f, 1f));
            _entity.AddComponent(new Player());
            _entity.AddComponent(new Health(200));
            _entity.AddComponent(new Sprite(Color4.White, Assets.GetTexture(@"TileMap.TilesetImages.DungeonTileSet.png"), RenderLayer.Front, true));

            _dungeonScene.AddSystem(new PlayerMovementSystem("Move"));
            _dungeonScene.AddSystem(new PhysicsSystem("Physics"));
            _dungeonScene.AddSystem(new CollisionSystem("Collision"));
            // _dungeonScene.AddSystem(new TileRenderSystem("TileRenderSystem", tileMap));
            _dungeonScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            //_dungeonScene.AddSystem(new SpriteRenderSystem("SpriteRenderSystem"));
            //_dungeonScene.AddSystem(new ColliderRenderSystem("CollisionSystem"));
            _dungeonScene.AddSystem(new CameraSystem("CameraSystem"));
            _dungeonScene.AddSystem(new SimpleAISystem("SimpleAISystem"));
            _dungeonScene.AddSystem(new Renderer("Renderer"));
            _dungeonScene.SetTileMap(tileMap);
            _dungeonScene.AddEntity(_entity);

            //Village Scene
            TileMap villageTileMap = new TileMap("VillageTileMap.tmx", false);
            _villageScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            _villageScene.AddSystem(new CollisionSystem("Collision"));
            _villageScene.AddSystem(new AnimatedTileSystem("AnimatedTileSystem"));
            _villageScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _villageScene.AddSystem(new CameraSystem("CameraSystem"));
            _villageScene.AddSystem(new HealthSystem("HealthSystem"));
            _villageScene.AddSystem(new Renderer("Renderer"));
            //_villageScene.AddSystem(new TileRenderSystem("TileRenderSystem", villageTileMap));
            //_villageScene.AddSystem(new ColliderRenderSystem("CollisionSystem"));
            //_villageScene.AddSystem(new AnimatedTileRenderSystem("AnimatedTileRenderSystem", villageTileMap));
            //_villageScene.AddSystem(new SpriteRenderSystem("SpriteRenderSystem"));
            _villageScene.SetTileMap(villageTileMap);
            _villageScene.AddEntity(_entity);
        }

        private void LoadResources()
        {
            Assets.GetShader("shader");
            Assets.AddSpriteSheet(
                "HenksFont.png",
                new SpriteSheet(Color4.White, Assets.GetTexture("HenksFont.png"), 5, 7, (10 * 10) - 5));
        }

        private void UpdateFrame(FrameEventArgs args)
        {
            foreach (MouseHandler.ClickedMouseButton button in MouseHandler.ClickedMouseButtons)
            {
                if (button.Button == MouseButton.Middle)
                {
                    SceneLoader.LoadScene("VillageScene");
                }

                if (button.Button == MouseButton.Button4)
                {
                    Manager.GetInstance().RemoveEntity(_entity);
                }

                if (button.Button == MouseButton.Button5)
                {
                    Manager.GetInstance().AddEntity(_entity);
                }
            }

            if (KeyHandler.IsPressed(Keys.V))
            {
                SceneLoader.LoadScene("DungeonScene");
            }

            Manager.GetInstance().Update((float)args.Time);
            DebugPrinter.PrintToConsole((float)args.Time);
            MouseHandler.ClickedMouseButtons.Clear();
        }

        private void RenderFrame(FrameEventArgs args)
        {
            _fps.SetFps((float)args.Time);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Manager.GetInstance().Render();

            void TextRendering()
            {
                Texture2D fontImage = Assets.GetTexture("HenksFont.png");
                GL.Enable(EnableCap.Texture2D);

                GL.Enable(EnableCap.Blend);
                GL.BindTexture(TextureTarget.Texture2D, fontImage.Handle);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)OpenTK.Graphics.OpenGL.TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)OpenTK.Graphics.OpenGL.TextureMinFilter.Nearest);

                // tilesize
                float tileWidth = 5f;
                float tileHeight = 7f;

                float aspectRatio = tileWidth / tileHeight;
                Vector2 tileTextureSize = new Vector2(tileWidth / (float)fontImage.Width, tileHeight / (float)fontImage.Height);

                CreateASCIIMap();

                void CreateASCIIMap(char asciiStart = ' ', char asciiEnd = '~')
                {
                    Dictionary<char, Vector2> asciiToCoord = new Dictionary<char, Vector2>();

                    float rows = fontImage.Height / (float)tileHeight;
                    float colums = fontImage.Width / (float)tileWidth;

                    float deltaX = (1 / tileWidth) / (float)fontImage.Width;
                    float deltaY = (1 / tileHeight) / (float)fontImage.Height;

                    for (char c = asciiStart; c <= asciiEnd; c++)
                    {
                        int rowPos = (int)((c - asciiStart) / colums);
                        int colPos = (int)((c - asciiStart) % colums);
                        float x = tileTextureSize.X * colPos;
                        float y = tileTextureSize.Y * (rows - rowPos - 1);

                        // Line Fix
                        x += deltaX / 2;
                        y += deltaY / 2;
                        asciiToCoord.Add(c, new Vector2(x, y));
                    }

                    tileTextureSize.X -= deltaX;
                    tileTextureSize.Y -= deltaY;

                    string fps = ((int)(1 / (float)args.Time)).ToString();

                    // Draw marvin
                    string text = "Text rendering Test: \n" +
                        "New Line\n" +
                        "FPS: " + fps + "\n" +
                        "ABCDEFGHIJKLMNOPQRSTUVWXYZ\n" +
                        "abcdefghijklmnopqrstuvwxyz\n" +
                        "!\"#&$/<>'()@[]{}_-:;=+-*\n" +
                        "1234567890!?\n" +
                        "8th Line\n" +
                        "All ok!";

                    float spacing = 0;
                    float newLines = -2;
                    foreach (char c in text.ToCharArray())
                    {
                        if (c == '\n')
                        {
                            newLines += -1.1f;
                            spacing = 0;
                            continue;
                        }

                        Graphics.DrawTile(new Rect(new Vector2(spacing, newLines), new Vector2(aspectRatio, 1f)), new Rect(asciiToCoord[c], tileTextureSize));
                        spacing += aspectRatio + 0.1f;
                    }
                }
            }
        }
    }
}
