using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using Villeon.Helper;
using Villeon.Components;
using Villeon.Systems;
using OpenTK.Graphics.OpenGL;
using Zenseless.Resources;
using Zenseless.OpenTK;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Villeon
{
    public class Game
    {
        private Scene dungeonScene = new ("DungeonScene");
        private Scene villageScene = new ("VillageScene");

        public void Start()
        {
            GameWindow gameWindow = WindowCreator.CreateWindow();
            Init();

            SceneLoader.AddScene(dungeonScene);
            SceneLoader.AddScene(villageScene);

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
            // Platformer Scene
            TileMap tileMap = new TileMap("Level.tmx");
            IEntity entity = new Entity("Marin");
            entity.AddComponent(new Physics());
            entity.AddComponent(new Collider(new Vector2(5.0f, 5.0f), 0.5f, 0.5f));
            entity.AddComponent(new Transform(new Vector2(0.0f, 0.0f), 1.0f, 0.0f));
            entity.AddComponent(new Sprite(Color4.Cornsilk, new Vector2(1f, 1f)));
            entity.AddComponent(new Player());

            dungeonScene.AddSystem(new PlayerMovementSystem("Move"));
            dungeonScene.AddSystem(new PhysicsSystem("Physics"));
            dungeonScene.AddSystem(new CollisionSystem("Collision"));
            dungeonScene.AddSystem(new TileRenderSystem("TileRenderSystem", tileMap));
            dungeonScene.AddSystem(new ColliderRenderSystem("RenderSystem"));
            dungeonScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            dungeonScene.AddSystem(new SpriteRenderSystem("SpriteRenderSystem"));
            dungeonScene.AddSystem(new CameraSystem("CameraSystem"));
            dungeonScene.SetTileMap(tileMap);
            dungeonScene.AddEntity(entity);

            // Village Scene
            TileMap villageTileMap = new TileMap("VillageTileMap.tmx");
            villageScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            villageScene.AddSystem(new CollisionSystem("Collision"));
            villageScene.AddSystem(new TileRenderSystem("TileRenderSystem", villageTileMap));
            villageScene.AddSystem(new ColliderRenderSystem("TileRenderSystem"));
            villageScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            villageScene.AddSystem(new SpriteRenderSystem("SpriteRenderSystem"));
            villageScene.AddSystem(new CameraSystem("CameraSystem"));
            villageScene.SetTileMap(villageTileMap);
            villageScene.AddEntity(entity);
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

        Matrix4 refCameraMatrix = Matrix4.Identity;

        private void RenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Camera.Update();
            this.refCameraMatrix = Camera.GetMatrix();
            GL.LoadMatrix(ref refCameraMatrix);

            Manager.GetInstance().Render();
        }
    }
}
