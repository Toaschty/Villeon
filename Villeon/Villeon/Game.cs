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
using Villeon.Components;
using Villeon.DungeonGeneration;
using Villeon.ECS;
using Villeon.GUI;
using Villeon.Helper;
using Villeon.Render;
using Villeon.Systems;
using Villeon.Systems.Update;
using Villeon.Utils;
using Zenseless.OpenTK;
using Zenseless.Resources;

namespace Villeon
{
    public class Game
    {
        private Scene _mainMenuScene = new ("MainMenuScene");
        private Scene _loadingScene = new ("LoadingScene");
        private Scene _dungeonScene;
        private Scene _villageScene;
        private Scene _smithScene;
        private Scene _shopScene;
        private GameWindow _gameWindow;
        private Matrix4 _refCameraMatrix = Matrix4.Identity;
        private FPS _fps;

        public Game()
        {
            _gameWindow = WindowHelper.CreateWindow();
            TypeRegistry.SetupTypes();
            Assets.LoadRessources();
            _fps = new FPS(_gameWindow);
        }

        public void Start()
        {
            SceneLoader.AddScene(_mainMenuScene);
            SceneLoader.AddScene(_loadingScene);
            SceneLoader.SetActiveScene("MainMenuScene");

            // Setup scenes
            SetupMainMenuScene();
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
            _dungeonScene.AddEntity(floor);

            // Ladder
            IEntity ladder = new Entity(new Transform(new Vector2(42, 0), 1f, 0), "Ladder");
            ladder.AddComponent(new Trigger(TriggerLayerType.LADDER, 2f, 100f));
            ladder.AddComponent(new Ladder(true));
            _dungeonScene.AddEntity(ladder);

            //SpawnDungeon();
        }

        private void SpawnDungeon()
        {
            // Spawn Dungeon
            LevelGeneration lvlGen = new LevelGeneration();
            lvlGen.GenSolutionPath();
            RoomGeneration roomGen = new RoomGeneration(lvlGen.StartRoomX, lvlGen.StartRoomY, lvlGen.EndRoomX, lvlGen.EndRoomY, lvlGen.RoomModels);
            for (int i = 0; i < lvlGen.RoomModels.GetLength(0); i++)
            {
                for (int j = 0; j < lvlGen.RoomModels.GetLength(1); j++)
                {
                    Console.Write(lvlGen.RoomModels[i, j].RoomType);
                }

                Console.WriteLine();
            }

            IEntity entity;
            for (int i = 0; i < lvlGen.RoomModels.GetLength(0); i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    for (int j = 0; j < lvlGen.RoomModels.GetLength(1); j++)
                    {
                        for (int l = 1; l < 10; l++)
                        {
                            int x = 32 - k;
                            int y = 40 - l;

                            if (i == 1)
                                x -= 8;
                            else if (i == 2)
                                x -= 16;
                            else if (i == 3)
                                x -= 24;
                            else if (i == 4)
                                x -= 32;

                            if (j == 1)
                                y -= 9;
                            else if (j == 2)
                                y -= 18;
                            else if (j == 3)
                                y -= 27;
                            else if (j == 4)
                                y -= 38;

                            if (roomGen.RoomModels[i, j].RoomLayout[k, l] == "1")
                            {
                                entity = new Entity(new Transform(new Vector2(y, x), new Vector2(1f, 1f), 0f), i + " " + k + " " + j + " " + l);
                                entity.AddComponent(Assets.GetSpriteSheet("TileMap.TilesetImages.DungeonTileSet.png").GetSprite(63, SpriteLayer.Background, false));
                                entity.AddComponent(new Collider(new Vector2(0, 0), entity.GetComponent<Transform>(), 1f, 1f));
                                _dungeonScene.AddEntity(entity);
                            }
                            else if (roomGen.RoomModels[i, j].RoomLayout[k, l] == "2")
                            {
                                // different block
                                entity = new Entity(new Transform(new Vector2(y, x), new Vector2(1f, 1f), 0f), i + " " + k + " " + j + " " + l);
                                entity.AddComponent(Assets.GetSpriteSheet("TileMap.TilesetImages.DungeonTileSet.png").GetSprite(20, SpriteLayer.Background, false));
                                entity.AddComponent(new Collider(new Vector2(0, 0), entity.GetComponent<Transform>(), 1f, 1f));
                                _dungeonScene.AddEntity(entity);
                            }
                            else if (roomGen.RoomModels[i, j].RoomLayout[k, l] == "0")
                            {
                                // air
                                entity = new Entity(new Transform(new Vector2(y, x), new Vector2(1f, 1f), 0f), i + " " + k + " " + j + " " + l);
                                entity.AddComponent(Assets.GetSpriteSheet("TileMap.TilesetImages.DungeonTileSet.png").GetSprite(0, SpriteLayer.Background, false));
                                _dungeonScene.AddEntity(entity);
                            }
                            else if (roomGen.RoomModels[i, j].RoomLayout[k, l] == "L")
                            {
                                // Ladder
                                entity = new Entity(new Transform(new Vector2(y, x), new Vector2(1f, 1f), 0f), i + " " + k + " " + j + " " + l);
                                entity.AddComponent(Assets.GetSpriteSheet("TileMap.TilesetImages.DungeonTileSet.png").GetSprite(0, SpriteLayer.Background, false));
                                entity.AddComponent(new Trigger(TriggerLayerType.LADDER, 1f, 1f));
                                entity.AddComponent(new Ladder(true));
                                _dungeonScene.AddEntity(entity);
                            }
                            else if (roomGen.RoomModels[i, j].RoomLayout[k, l] == "P")
                            {
                                // Ladder top
                                entity = new Entity(new Transform(new Vector2(y, x), new Vector2(1f, 1f), 0f), i + " " + k + " " + j + " " + l);
                                entity.AddComponent(Assets.GetSpriteSheet("TileMap.TilesetImages.DungeonTileSet.png").GetSprite(0, SpriteLayer.Background, false));
                                entity.AddComponent(new Trigger(TriggerLayerType.LADDER, 1f, 1f));
                                entity.AddComponent(new Ladder(true));
                                _dungeonScene.AddEntity(entity);
                            }
                        }
                    }
                }
            }
        }

        private void InitWindowActions(GameWindow gameWindow)
        {
            gameWindow.KeyDown += KeyHandler.KeyDown;
            gameWindow.KeyUp += KeyHandler.KeyUp;
            gameWindow.MouseWheel += MouseHandler.MouseWheel;
            gameWindow.MouseDown += MouseHandler.MouseDown;
            gameWindow.MouseMove += MouseHandler.MouseMove;
            gameWindow.UpdateFrame += UpdateFrame;
            gameWindow.RenderFrame += RenderFrame;
            gameWindow.Resize += Resize;
        }

        private void AddPortalEntities()
        {
            IEntity villageToDungeon = new Entity(new Transform(Constants.VILLAGE_SPAWN_POINT + new Vector2(5, 0), 1f, 0f), "villageToDungeonPortal");
            villageToDungeon.AddComponent(new Trigger(TriggerLayerType.PORTAL, 1f, 2f));
            villageToDungeon.AddComponent(new Portal("DungeonScene", Constants.VILLAGE_SPAWN_POINT));
            _villageScene.AddEntity(villageToDungeon);

            IEntity dungeonToVillage = new Entity(new Transform(new Vector2(1f, 3f), 1f, 0f), "dungeonToVillagePortal");
            dungeonToVillage.AddComponent(new Trigger(TriggerLayerType.PORTAL, 1f, 4f));
            dungeonToVillage.AddComponent(new Portal("VillageScene", Constants.DUNGEON_SPAWN_POINT));
            _dungeonScene.AddEntity(dungeonToVillage);

            IEntity villageToSmith = new Entity(new Transform(Constants.TO_SMITH_PORTAL_POINT, 1f, 0f), "VillageToSmithPortal");
            villageToSmith.AddComponent(new Trigger(TriggerLayerType.PORTAL, 2f, 1f));
            villageToSmith.AddComponent(new Portal("SmithScene", Constants.TO_SMITH_PORTAL_POINT + new Vector2(1f, -1f)));
            _villageScene.AddEntity(villageToSmith);

            IEntity smithToVillage = new Entity(new Transform(Constants.FROM_SMITH_PORTAL_POINT, 1f, 0f), "SmithToVillagePortal");
            smithToVillage.AddComponent(new Trigger(TriggerLayerType.PORTAL, 2f, 1.5f));
            smithToVillage.AddComponent(new Portal("VillageScene", Constants.FROM_SMITH_PORTAL_POINT + new Vector2(1f, 2f)));
            _smithScene.AddEntity(smithToVillage);

            IEntity villageToShop = new Entity(new Transform(Constants.TO_SHOP_PORTAL_POINT, 1f, 0f), "VillageToShopPortal");
            villageToShop.AddComponent(new Trigger(TriggerLayerType.PORTAL, 2f, 1f));
            villageToShop.AddComponent(new Portal("ShopScene", Constants.TO_SHOP_PORTAL_POINT + new Vector2(1f, -1f)));
            _villageScene.AddEntity(villageToShop);

            IEntity shopToVillage = new Entity(new Transform(Constants.FROM_SHOP_PORTAL_POINT, 1f, 0f), "ShopToVillagePortal");
            shopToVillage.AddComponent(new Trigger(TriggerLayerType.PORTAL, 2f, 1.5f));
            shopToVillage.AddComponent(new Portal("VillageScene", Constants.FROM_SHOP_PORTAL_POINT + new Vector2(1f, 2f)));
            _shopScene.AddEntity(shopToVillage);
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
            player.AddComponent(new Collider(new Vector2(0f, 0f), transform, 1f, 1f));
            player.AddComponent(new DynamicCollider(player.GetComponent<Collider>()));
            player.AddComponent(new Trigger(TriggerLayerType.FRIEND | TriggerLayerType.PORTAL | TriggerLayerType.LADDER, new Vector2(0f, 0f), 1f, 2f));
            player.AddComponent(new Sprite(Assets.GetTexture("Sprites.Player.png"), SpriteLayer.Foreground, true));
            player.AddComponent(new Physics());
            player.AddComponent(new Effect());
            player.AddComponent(new Player());
            player.AddComponent(new Health(Constants.PLAYER_MAX_HEALTH));

            // Setup player animations
            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_right.png", 0.08f));
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
            player.AddComponent(new Sprite(Assets.GetTexture("Sprites.Player.png"), SpriteLayer.Foreground, true));
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

        private void SetupMainMenuScene()
        {
            // Show background image
            Entity villageRenderEntity = new Entity(new Transform(new Vector2(0, 0), 0.3f, 0f), "VillageRender");
            villageRenderEntity.AddComponent(new Sprite(Assets.GetTexture("Sprites.VillageRender.png"), SpriteLayer.Background, false));
            _mainMenuScene.AddEntity(villageRenderEntity);

            // Enable camera movement
            Entity cameraMovementEntity = new Entity(new Transform(Vector2.Zero, 1f, 0f), "CameraMovement");
            cameraMovementEntity.AddComponent(new AutoCameraMovement(new Vector2(115f, 70f), 50f, 0.02f));
            _mainMenuScene.AddEntity(cameraMovementEntity);

            // Main menu GUI Setup
            new MainMenuSetup();

            Entity selectorEntity = new Entity(new Transform(Vector2.Zero, 1f, 0f), "Selector");
            selectorEntity.AddComponent(new MainMenu());
            _mainMenuScene.AddEntity(selectorEntity);

            StateManager.InMenu = true;

            // Add required systems
            _mainMenuScene.AddSystem(new CameraSystem("CameraSystem"));
            _mainMenuScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));
            _mainMenuScene.AddSystem(new AutoCameraMovementSystem("MovingCamera"));
            _mainMenuScene.AddSystem(new MainMenuInputSystem("MainMenuInput"));
        }

        private void CreatePlayers()
        {
            _villageScene.AddEntity(CreateVillagePlayer(Constants.VILLAGE_SPAWN_POINT));
            _dungeonScene.AddEntity(CreateDungeonPlayer());
            _smithScene.AddEntity(CreateVillagePlayer(Constants.SMITH_SPAWN_POINT));
            _shopScene.AddEntity(CreateVillagePlayer(Constants.SHOP_SPAWN_POINT));
        }

        private void SetupLoadingScene()
        {
            // Setup GUI
            Text loadingText = new Text("Loading...", new Vector2(-9.5f, -5.15f), "Alagard", 0f, 1f, 0.3f);
            Array.ForEach(loadingText.GetEntities(), entity => Manager.GetInstance().AddEntityToScene(entity, "LoadingScene"));

            // Add required systems
            _loadingScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));

            // Function which handles loading
            _loadingScene.AddStartUpFunc(() =>
            {
                // Render one frame to show the loading screen
                RenderFrame(new FrameEventArgs(0f));

                // Handle the actual loading
                SetupDungeonScene();
                SetupVillageScene();
                SetupSmithScene();
                SetupShopScene();
                AddPortalEntities();
                SetupGUIEntities();
                CreatePlayers();
                DebuggingPlayground();
                GUIHandler.GetInstance().LoadGUI();

                // Switch scene if loading is done
                SceneLoader.SetActiveScene("VillageScene");
                return true;
            });
        }

        private void SetupSmithScene()
        {
            _smithScene = new Scene("SmithScene");
            SceneLoader.AddScene(_smithScene);

            TileMapDictionary smithTileMap = new TileMapDictionary("VillageSmith.tmx");
            _smithScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            _smithScene.AddSystem(new CollisionSystem("Collision"));
            _smithScene.AddSystem(new TriggerSystem("Trigger"));
            _smithScene.AddSystem(new PortalSystem("PortalSystem"));
            _smithScene.AddSystem(new CameraSystem("CameraSystem"));
            _smithScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));
            _smithScene.AddSystem(new PlayerAnimationControllerSystem("AnimationControllerSystem"));
            _smithScene.AddSystem(new AnimationSystem("AnimationSystem"));
            _smithScene.AddSystem(new GUIInputSystem("GUIInputSystem"));
            _smithScene.SetTileMap(smithTileMap, false);
        }

        private void SetupShopScene()
        {
            _shopScene = new Scene("ShopScene");
            SceneLoader.AddScene(_shopScene);

            TileMapDictionary shopTileMap = new TileMapDictionary("VillageShop.tmx");
            _shopScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            _shopScene.AddSystem(new CollisionSystem("Collision"));
            _shopScene.AddSystem(new TriggerSystem("Trigger"));
            _shopScene.AddSystem(new PortalSystem("PortalSystem"));
            _shopScene.AddSystem(new CameraSystem("CameraSystem"));
            _shopScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));
            _shopScene.AddSystem(new PlayerAnimationControllerSystem("AnimationControllerSystem"));
            _shopScene.AddSystem(new AnimationSystem("AnimationSystem"));
            _shopScene.AddSystem(new GUIInputSystem("GUIInputSystem"));
            _shopScene.SetTileMap(shopTileMap, false);
        }

        private void SetupVillageScene()
        {
            _villageScene = new Scene("VillageScene");
            SceneLoader.AddScene(_villageScene);

            TileMapDictionary villageTileMap = new TileMapDictionary("Village.tmx");
            _villageScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            _villageScene.AddSystem(new CollisionSystem("Collision"));
            _villageScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _villageScene.AddSystem(new TriggerSystem("Trigger"));
            _villageScene.AddSystem(new PortalSystem("PortalSystem"));
            _villageScene.AddSystem(new CameraSystem("CameraSystem"));
            _villageScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));
            _villageScene.AddSystem(new PlayerAnimationControllerSystem("AnimationControllerSystem"));
            _villageScene.AddSystem(new AnimationSystem("AnimationSystem"));
            _villageScene.AddSystem(new GUIInputSystem("GUIInputSystem"));
            _villageScene.SetTileMap(villageTileMap, false);
        }

        private void SetupDungeonScene()
        {
            _dungeonScene = new Scene("DungeonScene");
            SceneLoader.AddScene(_dungeonScene);

            TileMapDictionary tileMap = new TileMapDictionary("DungeonTileMap.tmx");
            _dungeonScene.AddSystem(new EffectSystem("Effects"));
            _dungeonScene.AddSystem(new PlayerMovementSystem("Move"));
            _dungeonScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _dungeonScene.AddSystem(new SimpleAISystem("SimpleAISystem"));
            _dungeonScene.AddSystem(new PhysicsSystem("Physics"));
            _dungeonScene.AddSystem(new TriggerSystem("Trigger"));
            _dungeonScene.AddSystem(new PortalSystem("PortalSystem"));
            _dungeonScene.AddSystem(new DamageSystem("DamageSystem"));
            _dungeonScene.AddSystem(new CollisionSystem("Collision"));
            _dungeonScene.AddSystem(new HealthSystem("Health"));
            _dungeonScene.AddSystem(new CameraSystem("CameraSystem"));
            _dungeonScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));
            _dungeonScene.AddSystem(new AnimationSystem("AnimationSystem"));
            _dungeonScene.AddSystem(new LadderSystem("LadderSystem"));
            _dungeonScene.AddSystem(new HealthbarSystem("PlayerHealthbar", Constants.PLAYER_MAX_HEALTH));
            _dungeonScene.SetTileMap(tileMap, true);
        }

        private void SetupGUIEntities()
        {
            // Menu Buttons - Village
            GUI.Image dungeon_button = new GUI.Image("Dungeon_Button.png", new Vector2(-9f, -5f), new Vector2(0.3f));
            GUI.Image map_button = new GUI.Image("Map_Button.png", new Vector2(-7.5f, -5f), new Vector2(0.3f));
            GUI.Image equipment_button = new GUI.Image("Equipment_Button.png", new Vector2(-6f, -5f), new Vector2(0.3f));
            GUI.Image inventar_button = new GUI.Image("Inventar_Button.png", new Vector2(-4.5f, -5f), new Vector2(0.3f));

            _villageScene.AddEntities(dungeon_button.Entity, map_button.Entity, equipment_button.Entity, inventar_button.Entity);

            // Menu View - Village
            Entity guiHandlerEntity = new Entity("GuiHandler");
            guiHandlerEntity.AddComponent(GUIHandler.GetInstance());
            _villageScene.AddEntity(guiHandlerEntity);
        }

        private static int FrameCount = 0;
        private static Stopwatch stopwatch = new Stopwatch();

        private void UpdateFrame(FrameEventArgs args)
        {
            _fps!.SetFps((float)args.Time);
            Time.SetTime((float)args.Time);
            Manager.GetInstance().Update((float)args.Time);
            MouseHandler.ClickedMouseButtons.Clear();
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
