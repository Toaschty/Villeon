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
using Villeon.Systems.Render;
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
            _entity = new Entity("Marin");
            _entity.AddComponent(new Physics());
            _entity.AddComponent(new Collider(new Vector2(0.0f, 0.0f), new Vector2(35.0f, 35.0f), 0.5f, 0.5f));
            _entity.AddComponent(TriggerBuilder.Build(TriggerID.PLAYER));
            _entity.AddComponent(new Sprite(Color4.Cornsilk, new Vector2(1f, 1f)));
            _entity.AddComponent(new Player());
            _entity.AddComponent(new Health(200));

            _dungeonScene.AddEntity(_entity);
            _dungeonScene.AddSystem(new PlayerMovementSystem("Move"));
            _dungeonScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _dungeonScene.AddSystem(new SimpleAISystem("SimpleAISystem"));

            _dungeonScene.AddSystem(new PhysicsSystem("Physics"));
            _dungeonScene.AddSystem(new CollisionSystem("Collision"));
            _dungeonScene.AddSystem(new TriggerSystem("Trigger"));

            _dungeonScene.AddSystem(new HealthSystem("Health"));

            _dungeonScene.AddSystem(new CameraSystem("CameraSystem"));

            _dungeonScene.AddSystem(new TileRenderSystem("TileRenderSystem", tileMap));
            _dungeonScene.AddSystem(new SpriteRenderSystem("SpriteRenderSystem"));
            _dungeonScene.AddSystem(new ColliderRenderSystem("CollisionSystem"));
            _dungeonScene.AddSystem(new TriggerRenderSystem("TriggerRenderer"));
            _dungeonScene.SetTileMap(tileMap);

            // Village Scene
            TileMap villageTileMap = new TileMap("VillageTileMap.tmx", false);
            _villageScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            _villageScene.AddSystem(new CollisionSystem("Collision"));
            _villageScene.AddSystem(new TileRenderSystem("TileRenderSystem", villageTileMap));

            _villageScene.AddSystem(new ColliderRenderSystem("CollisionSystem"));
            _villageScene.AddSystem(new AnimatedTileSystem("AnimatedTileSystem"));
            _villageScene.AddSystem(new AnimatedTileRenderSystem("AnimatedTileRenderSystem", villageTileMap));

            _villageScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _villageScene.AddSystem(new SpriteRenderSystem("SpriteRenderSystem"));
            _villageScene.AddSystem(new CameraSystem("CameraSystem"));
            _villageScene.AddSystem(new HealthSystem("HealthSystem"));
            _villageScene.SetTileMap(villageTileMap);
            _villageScene.AddEntity(_entity);
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
                    SceneLoader.LoadScene("DungeonScene");
            }

            if (KeyHandler.IsPressed(Keys.V))
                SceneLoader.LoadScene("VillageScene");

            if (KeyHandler.IsPressed(Keys.G))
                Manager.GetInstance().RemoveComponent<Physics>(_entity);

            Manager.GetInstance().Update((float)args.Time);
            DebugPrinter.PrintToConsole((float)args.Time);
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
