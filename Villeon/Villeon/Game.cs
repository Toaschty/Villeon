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
using Villeon.Helper;
using Villeon.Systems;
using Zenseless.OpenTK;
using Zenseless.Resources;

namespace Villeon
{
    public class Game
    {
        private Scene _dungeonScene = new ("DungeonScene");
        private Scene _villageScene = new ("VillageScene");

        private Matrix4 _refCameraMatrix = Matrix4.Identity;

        public void Start()
        {
            GameWindow gameWindow = WindowCreator.CreateWindow();
            Init();

            SceneLoader.AddScene(_dungeonScene);
            SceneLoader.AddScene(_villageScene);

            // Load scene
            // SceneLoader.LoadScene(dungeonScene);
            SceneLoader.LoadScene("VillageScene");

            // Enable Texturing
            GL.Enable(EnableCap.Texture2D);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

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
            ComponentTypes.Init();

            // Platformer Scene
            TileMap tileMap = new TileMap("Level.tmx", true);
            IEntity entity = new Entity("Marin");
            entity.AddComponent(new Physics());
            entity.AddComponent(new Collider(new Vector2(5.0f, 5.0f), 0.5f, 0.5f));
            entity.AddComponent(new Sprite(Color4.Cornsilk, new Vector2(1f, 1f)));
            entity.AddComponent(new Player());

            _dungeonScene.AddSystem(new PlayerMovementSystem("Move"));
            _dungeonScene.AddSystem(new PhysicsSystem("Physics"));
            _dungeonScene.AddSystem(new CollisionSystem("Collision"));
            _dungeonScene.AddSystem(new TileRenderSystem("TileRenderSystem", tileMap));
            _dungeonScene.AddSystem(new ColliderRenderSystem("RenderSystem"));
            _dungeonScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _dungeonScene.AddSystem(new SpriteRenderSystem("SpriteRenderSystem"));
            _dungeonScene.AddSystem(new CameraSystem("CameraSystem"));
            _dungeonScene.SetTileMap(tileMap);
            _dungeonScene.AddEntity(entity);

            // Village Scene
            TileMap villageTileMap = new TileMap("VillageMap.tmx", false);
            _villageScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            _villageScene.AddSystem(new CollisionSystem("Collision"));
            _villageScene.AddSystem(new TileRenderSystem("TileRenderSystem", villageTileMap));
            _villageScene.AddSystem(new ColliderRenderSystem("TileRenderSystem"));
            _villageScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _villageScene.AddSystem(new SpriteRenderSystem("SpriteRenderSystem"));
            _villageScene.AddSystem(new CameraSystem("CameraSystem"));
            _villageScene.SetTileMap(villageTileMap);
            _villageScene.AddEntity(entity);
        }

        private void UpdateFrame(FrameEventArgs args)
        {
            foreach (MouseHandler.ClickedMouseButton button in MouseHandler.ClickedMouseButtons)
            {
                if (button.Button == MouseButton.Middle)
                {
                    SceneLoader.LoadScene("VillageScene");
                }

                if (button.Button == MouseButton.Right)
                {
                    SceneLoader.LoadScene("DungeonScene");
                }
            }

            Manager.GetInstance().Update((float)args.Time);
            MouseHandler.ClickedMouseButtons.Clear();
        }

        private void RenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Camera.Update();
            _refCameraMatrix = Camera.GetMatrix();
            GL.LoadMatrix(ref _refCameraMatrix);

            Manager.GetInstance().Render();
        }
    }
}
