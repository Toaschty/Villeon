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
            Init();

            GameWindow gameWindow = WindowCreator.CreateWindow();
            gameWindow.KeyDown += KeyHandler.KeyDown;
            gameWindow.KeyUp += KeyHandler.KeyUp;
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
            // Create some Entity
            Signature signature = new();
            signature.Add<Collider>();
            List<IEntity> entities = new List<IEntity>();
            entities.Add(manager.CreateEntity("Peter", signature));
            entities[0].AddComponent(new Collider(new Box2(0.0f, 0.0f, 1.5f, 1.5f)));

            PlayerMovementSystem movement = new("Movement");
            manager.RegisterSystem(movement);

            RenderSystem renderSystem = new("RenderSystem");
            manager.RegisterSystem(renderSystem);
        }


        private void UpdateFrame(FrameEventArgs args)
        {
           Console.WriteLine("--------------------- UPDATE" + ++UpdateCalled + "---------------------- ");
            manager.Update();
           Console.WriteLine("------------------ UPDATE DONE -------------------- ");
        }

        private void RenderFrame(FrameEventArgs args)
        {
           Console.WriteLine("--------------------- RENDER" + ++RenderCalled + "---------------------- ");
            manager.Render();
           Console.WriteLine("------------------ RENDER DONE -------------------- ");
        }
    }
}
