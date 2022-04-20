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
        IManager manager = new Manager();
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
            GL.Viewport(0, 0, args.Width, args.Height);
        }

        private void Init()
        {
            // Register some System
            CollisionSystem collision = new("CollisionSystem");
            manager.RegisterSystem(collision);
            
            PlayerMovementSystem movement = new("Movement");
            manager.RegisterSystem(movement);

            SpriteRenderSystem spriteRenderSystem = new("SpriteSystem");
            manager.RegisterSystem(spriteRenderSystem);

            // Create some Entity
            Signature signature = new();
            signature.Add<Transform>();
            signature.Add<Collider>();
            signature.Add<Physics>();
            signature.Add<SpriteDrawable>();

            IEntity peter = manager.CreateEntity("Peter", signature);
            peter.AddComponent(new Transform(new Vector2(0.0f, 0.0f), 1.0f, 0.0f));
            peter.AddComponent(new SpriteDrawable(Color4.AliceBlue, new Vector2(0.5f, 0.5f)));
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
