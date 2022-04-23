﻿using System;
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
        private EntitySpawner _spawner;

        public Game()
        {
            _spawner = new EntitySpawner(manager);
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
            IEntity entity = new Entity("Marin");
            entity.AddComponent(new Physics());
            entity.AddComponent(new Collider(new Vector2(5.0f, 5.0f), 1.0f, 1.0f));
            entity.AddComponent(new Transform(new Vector2(0.0f, 0.0f), 1.0f, 0.0f));
            entity.AddComponent(new Player());
            manager.AddEntity(entity);

            IEntity ground = new Entity("Ground");
            ground.AddComponent(new Collider(new Vector2(0.0f, 1.0f), 55.0f, 1.0f));
            ground.AddComponent(new Transform(new Vector2(0.0f, 0.0f), 25.0f, 1.0f));
            manager.AddEntity(ground);

            // Spawn Ground
            IEntity roof = new Entity("Roof");
            roof.AddComponent(new Collider(new Vector2(0.0f, 16.0f), 55.0f, 2.0f));
            roof.AddComponent(new Transform(new Vector2(0.0f, 0.0f), 25.0f, 1.0f));
            manager.AddEntity(roof);

            IEntity block = new Entity("Block1");
            block.AddComponent(new Collider(new Vector2(5.0f, 2.5f), 2.0f, 2.0f));
            block.AddComponent(new Transform(new Vector2(0.0f, 0.0f), 25.0f, 1.0f));
            manager.AddEntity(block);

            IEntity block2 = new Entity("Block2");
            block2.AddComponent(new Collider(new Vector2(10.0f, 3.5f), 2.0f, 2.0f));
            block2.AddComponent(new Transform(new Vector2(0.0f, 0.0f), 25.0f, 1.0f));
            manager.AddEntity(block2);

            IEntity block3 = new Entity("Block3");
            block3.AddComponent(new Collider(new Vector2(30.0f, 3.5f), 4.0f, 2.0f));
            block3.AddComponent(new Transform(new Vector2(0.0f, 0.0f), 25.0f, 1.0f));
            manager.AddEntity(block3);



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

            // Events..
        }


        private void UpdateFrame(FrameEventArgs args)
        {
            manager.Update(args.Time);

            // Spawner logic
            if (MouseHandler.MouseClicks.Count > 0)
            {
                Console.Write("Pop");
                _spawner.Spawn(MouseHandler.MouseClicks.Pop());
            }

        }

        private void RenderFrame(FrameEventArgs args)
        {
            manager.Render();
        }
    }
}
