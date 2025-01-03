﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation;
using Villeon.Generation.DungeonGeneration;
using Villeon.GUI;
using Villeon.Helper;
using Villeon.Render;
using Villeon.Systems;
using Villeon.Systems.RenderSystems;
using Villeon.Systems.Update;
using Villeon.Utils;
using Zenseless.OpenTK;
using Zenseless.Resources;

namespace Villeon
{
    public class Game
    {
        private GameWindow _gameWindow;
        private Matrix4 _refCameraMatrix = Matrix4.Identity;
        private FPS _fps;

        public Game()
        {
            _gameWindow = WindowHelper.CreateWindow();
            TypeRegistry.SetupTypes();
            ItemDrops.SetupDrops();

            Fonts.AddFont("Alagard", new Font(Color4.White, Asset.GetTexture("Fonts.Alagard.png"), "Fonts.Alagard.json"));
            Fonts.AddFont("Alagard_Thin", new Font(Color4.White, Asset.GetTexture("Fonts.Alagard_Thin.png"), "Fonts.Alagard_Thin.json"));

            Asset.LoadRessources();

            _fps = new FPS(_gameWindow);
        }

        public void Start()
        {
            SceneLoader.AddScene(Scenes.MainMenuScene);
            SceneLoader.AddScene(Scenes.LoadingScene);
            SceneLoader.SetActiveScene("MainMenuScene");

            // Setup scenes
            Scenes.SetupMainMenuScene();
            SetupLoadingScene();

            // Init and Start the Window
            InitWindowActions(_gameWindow);
            _gameWindow.Run();
        }

        private void InitWindowActions(GameWindow gameWindow)
        {
            gameWindow.KeyDown += KeyHandler.KeyDown;
            gameWindow.KeyUp += KeyHandler.KeyUp;
            gameWindow.MouseWheel += MouseWheelHandler.MouseWheel;
            gameWindow.MouseDown += MouseHandler.MouseDown;
            gameWindow.MouseMove += MouseHandler.MouseMove;
            gameWindow.UpdateFrame += UpdateFrame;
            gameWindow.RenderFrame += RenderFrame;
            gameWindow.Resize += Resize;
        }

        private void Resize(ResizeEventArgs args)
        {
            Camera.Resize(args.Width, args.Height);
            GL.Viewport(0, 0, args.Width, args.Height);
        }

        private void SetupLoadingScene()
        {
            // Setup GUI
            Text loadingText = new Text("Loading...", new Vector2(-9.5f, -5.15f), "Alagard", 0f, 1f, 0.3f);
            Array.ForEach(loadingText.GetEntities(), entity => Manager.GetInstance().AddEntityToScene(entity, "LoadingScene"));

            // Add required systems
            Scenes.LoadingScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));

            // Function which handles loading
            Scenes.LoadingScene.AddStartUpFunc(() =>
            {
                // Render one frame to show the loading screen
                RenderFrame(new FrameEventArgs(0f));

                // Handle the actual loading
                Scenes.SetupDungeonScene();
                Scenes.SetupTutorialScene();
                Scenes.SetupVillageScene();
                Scenes.SetupSmithScene();
                Scenes.SetupShopScene();
                Scenes.SetupPortalEntities();
                Scenes.SetupBossScene();
                SetupGUIEntities();
                CreatePlayers();
                GUIHandler.GetInstance().LoadGUI();

                // Load NPCs from file
                NPCLoader.LoadNpcs("TutorialScene");
                NPCLoader.LoadNpcs("VillageScene");
                NPCLoader.LoadNpcs("SmithScene");
                NPCLoader.LoadNpcs("ShopScene");

                // Load Unlockable NPCs
                NPCLoader.SpawnUnlockableNPCs();

                // Switch scene if loading is done
                if (Stats.GetInstance().Progress == 0)
                    SceneLoader.SetActiveScene("TutorialScene");
                else
                    SceneLoader.SetActiveScene("VillageScene");

                return true;
            });
        }

        private void CreatePlayers()
        {
            Scenes.TutorialScene.AddEntity(Players.CreateVillagePlayer(Constants.TUTORIAL_SPAWN_POINT));
            Scenes.VillageScene.AddEntity(Players.CreateVillagePlayer(Constants.VILLAGE_SPAWN_POINT));
            Scenes.DungeonScene.AddEntity(Players.CreateDungeonPlayer(Constants.DUNGEON_SPAWN_POINT));
            Scenes.SmithScene.AddEntity(Players.CreateVillagePlayer(Constants.SMITH_SPAWN_POINT));
            Scenes.ShopScene.AddEntity(Players.CreateVillagePlayer(Constants.SHOP_SPAWN_POINT));
        }

        private void SetupGUIEntities()
        {
            // Overlay - Tutorial
            TutorialOverlay tutorialOverlay = new TutorialOverlay();
            Scenes.TutorialScene.AddEntities(tutorialOverlay.GetEntities());

            // Overlay - Village
            VillageOverlay villageOverlay = new VillageOverlay();
            Scenes.VillageScene.AddEntities(villageOverlay.GetEntities());
            Scenes.ShopScene.AddEntities(villageOverlay.GetEntities());
            Scenes.SmithScene.AddEntities(villageOverlay.GetEntities());

            // Overlay - Dungeon
            DungeonOverlay dungeonOverlay = new DungeonOverlay();
            Scenes.DungeonScene.AddEntities(dungeonOverlay.GetEntities());

            // Menu View - Village
            Entity guiHandlerEntity = new Entity("GuiHandler");
            guiHandlerEntity.AddComponent(GUIHandler.GetInstance());

            Scenes.VillageScene.AddEntity(guiHandlerEntity);
            Scenes.ShopScene.AddEntity(guiHandlerEntity);
            Scenes.SmithScene.AddEntity(guiHandlerEntity);
            Scenes.DungeonScene.AddEntity(guiHandlerEntity);
        }

        private void UpdateFrame(FrameEventArgs args)
        {
            _fps!.SetFps((float)args.Time);
            Time.SetTime((float)args.Time);
            Manager.GetInstance().Update((float)args.Time);
            MouseHandler.Clear();
            KeyHandler.UpdateKeys();
        }

        private void RenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Manager.GetInstance().Render();
            _gameWindow.SwapBuffers();
        }
    }
}
