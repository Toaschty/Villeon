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

namespace Villeon
{
    public class Game
    {
        Manager manager = new();

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
            gameWindow.Run();
        }

        private void Init()
        {
            // Register some System
            CollisionSystem collision = new("CollisionSystem");
            manager.RegisterSystem(collision);

            PlayerMovementSystem movement = new("Movement");
            manager.RegisterSystem(movement);

            // Create some Entity
            Signature signature = new();
            signature.Add<Transform>();
            signature.Add<Collider>();

            Entity peter = manager.CreateEntity("Peter", signature);
            peter.AddComponent(new Transform(new Vector2(10.0f, 5.0f), 1.0f, 0.0f));

            signature.Add<Physics>();

            Entity oli = manager.CreateEntity("Oli", signature);
            oli.AddComponent(new Transform(new Vector2(1.0f, 5.0f), 1.0f, 0.0f));
        }


        private void UpdateFrame(FrameEventArgs args)
        {
            Console.WriteLine("--------------------- Start ---------------------- ");
            manager.Update();
            Console.WriteLine("--------------------- Done ----------------------- ");
        }

        private void RenderFrame(FrameEventArgs args)
        {
            // Implement IRenderSystem
            // https://gamedev.stackexchange.com/questions/181304/ecs-as-part-of-the-rendering-pipeline-of-an-engine
        }
    }
}
