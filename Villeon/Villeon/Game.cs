using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Villeon.Helper;

using Villeon.Component;

namespace Villeon
{
    public class Game
    {
        World world;
        Renderer renderer;

        public Game()
        {
            world = new World();
            renderer = new Renderer();
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
            world.AddEntity(new Player(new Input()));
            world.AddEntity(new Block());
        }


        private void UpdateFrame(FrameEventArgs args)
        {
            world.Update();
        }

        private void RenderFrame(FrameEventArgs args)
        {
            world.Draw(renderer);
        }
    }
}
