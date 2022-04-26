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

namespace Villeon
{
    public class Game
    {
        // private EntitySpawner _spawner;

        private Scene dungeonScene = new();
        private Scene villageScene = new();

        public void Start()
        {
            GameWindow gameWindow = WindowCreator.CreateWindow();

            Init();

            // Load scene
            //SceneLoader.LoadScene(dungeonScene);
            SceneLoader.LoadScene(villageScene);

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
            TileMap tileMap = new TileMap("Level.tmx", true);
            IEntity entity = new Entity("Marin");
            entity.AddComponent(new Physics());
            entity.AddComponent(new Collider(new Vector2(5.0f, 5.0f), 0.5f, 0.5f));
            entity.AddComponent(new Transform(new Vector2(0.0f, 0.0f), 1.0f, 0.0f));
            entity.AddComponent(new Player());
            entity.AddComponent(new Tile(
                new Vector2(5.0f, 5.0f),
                0.0f,
                0.0f,
                new Tile.TileSetStruct
                {
                    Texture2D = Texture2DLoader.Load(ResourceLoader.LoadContentAsStream("TileMap.TilesetImages.tiles.png")).Handle,
                    TileWidth = 1,
                    TileHeight = 1,
                }));
            dungeonScene.AddSystem(new PlayerMovementSystem("Move"));
            dungeonScene.AddSystem(new PhysicsSystem("Physics"));
            dungeonScene.AddSystem(new CollisionSystem("Collision"));
            dungeonScene.AddSystem(new TileRenderSystem("TileRenderSystem", tileMap));
            dungeonScene.AddSystem(new ColliderRenderSystem("RenderSystem"));
            dungeonScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            dungeonScene.AddEntity(entity);
            dungeonScene.SetTileMap(tileMap);

            // Village Scene
            TileMap villageTileMap = new TileMap("VillageMap.tmx", false);
            villageScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            villageScene.AddSystem(new CollisionSystem("Collision"));
            villageScene.AddSystem(new TileRenderSystem("TileRenderSystem", villageTileMap));
            villageScene.AddSystem(new ColliderRenderSystem("TileRenderSystem"));
            villageScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            villageScene.SetTileMap(villageTileMap);
            villageScene.AddEntity(entity);
        }

        private void UpdateFrame(FrameEventArgs args)
        {
            Console.Write("Entities: " + Manager.GetInstance()._entities.Count + "       UpdateSystems: " + Manager.GetInstance()._updateSystems.Count + "      RenderSystems: " + Manager.GetInstance()._renderSystems.Count() + "                                           \r");
            if (MouseHandler.ClickedMouseButtons.Count > 0)
            {
                if (MouseHandler.ClickedMouseButtons.Contains(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Middle))
                {
                    MouseHandler.ClickedMouseButtons.Dequeue();
                    SceneLoader.UnloadScene(dungeonScene);
                    SceneLoader.LoadScene(villageScene);
                }

                if (MouseHandler.ClickedMouseButtons.Contains(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Right))
                {
                    MouseHandler.ClickedMouseButtons.Dequeue();
                    SceneLoader.UnloadScene(villageScene);
                    SceneLoader.LoadScene(dungeonScene);
                }
            }
            Manager.GetInstance().Update(args.Time);
        }

        private void RenderFrame(FrameEventArgs args)
        {
            Manager.GetInstance().Render();
        }
    }
}
