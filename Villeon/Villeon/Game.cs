using System;
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

        private void DebuggingPlayground()
        {
            // floor
            IEntity floor = new Entity(new Transform(new Vector2(-20, -2), 1f, 0f), "Floor");
            floor.AddComponent(new Collider(new Vector2(0), new Transform(new Vector2(-20, -2), 1f, 0f), 100f, 1f));
            Scenes.DungeonScene.AddEntity(floor);

            // GIVE PLAYER ITEMS
            InventoryMenu.GetInstance().AddItems(ItemLoader.GetItem("Rock"), 512);
            InventoryMenu.GetInstance().AddItems(ItemLoader.GetItem("HealthPotion"), 8);
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

        private IEntity CreateDungeonPlayer()
        {
            IEntity player;
            Transform transform = new Transform(Constants.DUNGEON_SPAWN_POINT, 0.5f, 0f);
            player = new Entity(transform, "DungeonMarin");
            player.AddComponent(new Collider(new Vector2(0f, 0f), transform, 1f, 1.5f));
            player.AddComponent(new DynamicCollider(player.GetComponent<Collider>()));
            player.AddComponent(new Trigger(TriggerLayerType.FRIEND | TriggerLayerType.PORTAL | TriggerLayerType.LADDER | TriggerLayerType.MOBDROP, new Vector2(0f, 0f), 1f, 2f));
            player.AddComponent(new Sprite(Asset.GetTexture("Sprites.Player.png"), SpriteLayer.Foreground, true));
            player.AddComponent(new Physics());
            player.AddComponent(new Effect());
            player.AddComponent(new Player());
            player.AddComponent(new Light(Color4.White, -12, 4f, new Vector2(0.5f, 1f)));
            player.AddComponent(new Health(Constants.PLAYER_MAX_HEALTH));

            // Setup player animations
            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.d_player_idle.png", 0.5f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.d_player_walk_left.png", 0.08f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.d_player_walk_right.png", 0.08f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.d_player_jump_left.png", 0.2f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.d_player_jump_right.png", 0.2f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.d_player_fall_left.png", 0.1f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.d_player_fall_right.png", 0.1f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.d_player_climb.png", 0.08f));
            player.AddComponent(animController);
            return player;
        }

        private IEntity CreateVillagePlayer(Vector2 spawnPoint)
        {
            IEntity player;
            Transform transform = new Transform(spawnPoint, 0.25f, 0f);
            player = new Entity(transform, "VillageMarin");
            player.AddComponent(new Collider(new Vector2(0f, 0f), transform, 0.5f, 0.5f));
            player.AddComponent(new DynamicCollider(player.GetComponent<Collider>()));
            player.AddComponent(new Trigger(TriggerLayerType.FRIEND | TriggerLayerType.PORTAL, new Vector2(0f, 0f), 0.5f, 0.5f));
            player.AddComponent(new Sprite(Asset.GetTexture("Sprites.Player.png"), SpriteLayer.Foreground, true));
            player.AddComponent(new Light(Color4.White));
            player.AddComponent(new Player());

            // Setup player animations
            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_idle.png", 0.5f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_up.png", 0.08f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_down.png", 0.08f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_left.png", 0.08f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_right.png", 0.08f));
            player.AddComponent(animController);
            return player;
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
                Scenes.SetupVillageScene();
                Scenes.SetupSmithScene();
                Scenes.SetupShopScene();
                Scenes.SetupPortalEntities();
                SetupGUIEntities();
                CreatePlayers();
                DebuggingPlayground();
                GUIHandler.GetInstance().LoadGUI();

                // Load NPCs from file
                NPCLoader.LoadNpcs("VillageScene");
                NPCLoader.LoadNpcs("SmithScene");
                NPCLoader.LoadNpcs("ShopScene");

                // Switch scene if loading is done
                SceneLoader.SetActiveScene("VillageScene");
                return true;
            });
        }

        private void CreatePlayers()
        {
            Scenes.VillageScene.AddEntity(CreateVillagePlayer(Constants.VILLAGE_SPAWN_POINT));
            Scenes.DungeonScene.AddEntity(CreateDungeonPlayer());
            Scenes.SmithScene.AddEntity(CreateVillagePlayer(Constants.SMITH_SPAWN_POINT));
            Scenes.ShopScene.AddEntity(CreateVillagePlayer(Constants.SHOP_SPAWN_POINT));
        }

        private void SetupGUIEntities()
        {
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
