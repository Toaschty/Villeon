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

namespace Villeon
{
    public class Game
    {
        Manager manager = new Manager();
        int UpdateCalled = 0;
        int RenderCalled = 0;

        public Game()
        {
        }

        public void Start()
        {
            GameWindow gameWindow = WindowCreator.CreateWindow();

            Init();

            // Enable Texturing
            GL.Enable(EnableCap.Texture2D);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            gameWindow.KeyDown += KeyHandler.KeyDown;
            gameWindow.KeyUp += KeyHandler.KeyUp;
            gameWindow.MouseWheel += MouseHandler.MouseWheel;
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
            IEntity entity = new Entity("Marin");
            entity.AddComponent(new Physics());
            entity.AddComponent(new Collider(new Vector2(5.0f, 5.0f), 1.0f, 1.0f));
            entity.AddComponent(new Transform(new Vector2(0.0f, 0.0f), 1.0f, 0.0f));
            manager.AddEntity(entity);

            PlayerMovementSystem playerMovementSystem = new PlayerMovementSystem("Move");
            manager.RegisterSystem(playerMovementSystem);

            PhysicsSystem physicsSystem = new PhysicsSystem("Physics!");
            manager.RegisterSystem(physicsSystem);  

            CollisionSystem collisionSystem = new CollisionSystem("Collision!");
            manager.RegisterSystem(collisionSystem);
            
            // TileMap
            TileMap tileMap = new TileMap("Level.tmx", manager);

            TileRenderSystem tileRenderSystem = new("TileRenderSystem", tileMap);
            manager.RegisterSystem(tileRenderSystem);
            
            RenderSystem renderSystem = new("RenderSystem");
            manager.RegisterSystem(renderSystem);
        }


        private void UpdateFrame(FrameEventArgs args)
        {
            manager.Update(args.Time);
        }

        private void RenderFrame(FrameEventArgs args)
        {
            manager.Render();
        }
    }
}
