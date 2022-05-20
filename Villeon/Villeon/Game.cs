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
using Villeon.GUI;
using Villeon.Helper;
using Villeon.Render;
using Villeon.Systems;
using Villeon.Utils;
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
        private FPS? _fps;
        TextBox textBox;

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

            // Write some Text
            textBox = new TextBox("199", _entity.GetComponent<Transform>().Position + new Vector2(-3f, 3f), 1.1f, 1.1f, 1f);
            textBox.BindPositionTo(_entity);
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
            TileMapDictionary tileMap = new TileMapDictionary("DungeonTileMap.tmx");
            Transform transform = new Transform(new Vector2(1f, 1f), 1f, 0f);
            _entity = new Entity(transform, "Marin");
            _entity.AddComponent(new Physics());
            _entity.AddComponent(new Collider(new Vector2(0.5f, 0f), transform, 1f, 1f));
            _entity.AddComponent(TriggerBuilder.Build(TriggerID.PLAYER));
            _entity.AddComponent(new Player());
            _entity.AddComponent(new Health(200));
            _entity.AddComponent(new Sprite(Assets.GetTexture("Player.png"), SpriteLayer.Foreground, true));

            _dungeonScene.AddSystem(new PlayerMovementSystem("Move"));
            _dungeonScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _dungeonScene.AddSystem(new SimpleAISystem("SimpleAISystem"));
            _dungeonScene.AddSystem(new PhysicsSystem("Physics"));
            _dungeonScene.AddSystem(new TriggerSystem("Trigger"));
            _dungeonScene.AddSystem(new CollisionSystem("Collision"));
            _dungeonScene.AddSystem(new HealthSystem("Health"));
            _dungeonScene.AddSystem(new CameraSystem("CameraSystem"));
            _dungeonScene.AddSystem(new SpriteRenderer("SpriteRenderer", true));
            _dungeonScene.SetTileMap(tileMap, true);
            _dungeonScene.AddEntity(_entity);

            //Village Scene
            // TileMap villageTileMap = new TileMap("VillageTileMap.tmx", false);
            TileMapDictionary villageTileMap = new TileMapDictionary("VillageTileMap.tmx");
            _villageScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            _villageScene.AddSystem(new CollisionSystem("Collision"));
            _villageScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _villageScene.AddSystem(new CameraSystem("CameraSystem"));
            _villageScene.AddSystem(new HealthSystem("HealthSystem"));
            _villageScene.AddSystem(new SpriteRenderer("SpriteRenderer", true));

            _villageScene.SetTileMap(villageTileMap, false);
            _villageScene.AddEntity(_entity);
        }

        private void LoadResources()
        {
            Assets.GetShader("shader");
            Assets.AddSpriteSheet("HenksFont.png", new SpriteSheet(Color4.White, Assets.GetTexture("HenksFont.png"), 5, 7, (10 * 10) - 5));
        }

        private void UpdateFrame(FrameEventArgs args)
        {
            foreach (MouseHandler.ClickedMouseButton button in MouseHandler.ClickedMouseButtons)
            {
                if (button.Button == MouseButton.Middle)
                {
                    SceneLoader.LoadScene("VillageScene");
                }
            }

            if (KeyHandler.IsPressed(Keys.V))
            {
                SceneLoader.LoadScene("DungeonScene");
            }

            Manager.GetInstance().Update((float)args.Time);
            textBox.Update();
            MouseHandler.ClickedMouseButtons.Clear();
        }

        private void RenderFrame(FrameEventArgs args)
        {
            _fps.SetFps((float)args.Time);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Manager.GetInstance().Render();
        }
    }
}
