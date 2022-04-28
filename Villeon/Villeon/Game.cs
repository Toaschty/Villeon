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

        private IEntity entity;

        private float timeCounter;

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
            TypeRegistry.Init();

            // Platformer Scene
            TileMap tileMap = new TileMap("DungeonTileMap.tmx", true);
            entity = new Entity("Marin");
            entity.AddComponent(new Physics());
            entity.AddComponent(new Physics());
            entity.AddComponent(new Physics());
            entity.AddComponent(new Collider(new Vector2(0.0f, 0.0f), new Vector2(30.0f, 30.0f), 0.5f, 0.5f));
            entity.AddComponent(new Sprite(Color4.Cornsilk, new Vector2(1f, 1f)));
            entity.AddComponent(new Player());

            _dungeonScene.AddEntity(entity);
            _dungeonScene.AddSystem(new PlayerMovementSystem("Move"));
            _dungeonScene.AddSystem(new PhysicsSystem("Physics"));
            _dungeonScene.AddSystem(new CollisionSystem("Collision"));
            _dungeonScene.AddSystem(new TileRenderSystem("TileRenderSystem", tileMap));
            _dungeonScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _dungeonScene.AddSystem(new SpriteRenderSystem("SpriteRenderSystem"));
            _dungeonScene.AddSystem(new ColliderRenderSystem("CollisionSystem"));
            _dungeonScene.AddSystem(new CameraSystem("CameraSystem"));
            _dungeonScene.SetTileMap(tileMap);

            // Village Scene
            entity.RemoveComponent<Physics>();
            TileMap villageTileMap = new TileMap("VillageTileMap.tmx", false);
            _villageScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            _villageScene.AddSystem(new PhysicsSystem("Physics"));
            _villageScene.AddSystem(new CollisionSystem("Collision"));
            _villageScene.AddSystem(new TileRenderSystem("TileRenderSystem", villageTileMap));
            _villageScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _villageScene.AddSystem(new SpriteRenderSystem("SpriteRenderSystem"));
            _villageScene.AddSystem(new ColliderRenderSystem("CollisionSystem"));
            _villageScene.AddSystem(new CameraSystem("CameraSystem"));
            _villageScene.SetTileMap(villageTileMap);
            _villageScene.AddEntity(entity);
        }

        private void UpdateFrame(FrameEventArgs args)
        {
            DebugPrints((float)args.Time);

            foreach (MouseHandler.ClickedMouseButton button in MouseHandler.ClickedMouseButtons)
            {
                if (button.Button == MouseButton.Middle)
                    SceneLoader.LoadScene("VillageScene");

                if (button.Button == MouseButton.Right)
                    SceneLoader.LoadScene("DungeonScene");
            }

            if (KeyHandler.IsPressed(Keys.V))
                SceneLoader.LoadScene("VillageScene");


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

        private void DebugPrints(float time)
        {
            timeCounter += time;
            if (timeCounter > 0.1)
            {
                ClearConsole();
                Console.WriteLine("Entities: " + Manager.GetInstance().GetEntities().Count() + " Time: " + timeCounter);

                foreach (var system in Manager.GetInstance().GetUpdateSystems())
                {
                    Console.WriteLine("System: " + system + " Entities: " + system.Entities.Count);
                }

                foreach (var render in Manager.GetInstance().GetRenderSystems())
                {
                    Console.WriteLine("System: " + render + " Entities: " + render.Entities.Count);
                }

                timeCounter = 0;
            }

            void ClearConsole()
            {
                for (int i = 0; i < Console.WindowHeight; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");

                }

                Console.SetCursorPosition(0, 0);
            }
        }
    }
}
