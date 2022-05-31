using System;
using System.Collections.Generic;
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
        private Scene _dungeonScene = new ("DungeonScene");
        private Scene _villageScene = new ("VillageScene");
        private GameWindow _gameWindow;
        private Matrix4 _refCameraMatrix = Matrix4.Identity;
        private IEntity _player;
        private FPS _fps;

        public Game()
        {
            _gameWindow = WindowCreator.CreateWindow();
            TypeRegistry.SetupTypes();
            Assets.LoadRessources();
            _player = new Entity("Player not Set!");
            _fps = new FPS(_gameWindow);
        }

        public void Start()
        {
            SceneLoader.AddScene(_mainMenuScene);
            SceneLoader.AddScene(_dungeonScene);
            SceneLoader.AddScene(_villageScene);
            SceneLoader.SetActiveScene("MainMenuScene");

            SetupMainMenuScene();
            AddDungeonSystems();
            AddVillageSystems();
            AddPortalEntities();
            AddGUIEntities();
            CreatePlayerEntity();

            // Add player to scenes
            _villageScene.AddEntity(_player!);
            _dungeonScene.AddEntity(_player!);

            // Add testing entities before the window starts
            DebuggingPlayground();

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

            // Spawn Dungeon
            LevelGeneration lvlGen = new LevelGeneration();
            lvlGen.GenSolutionPath();
            RoomGeneration roomGen = new RoomGeneration(lvlGen.StartRoomX, lvlGen.StartRoomY, lvlGen.EndRoomX, lvlGen.EndRoomY, lvlGen.RoomModels);
            IEntity entity;
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    for (int j = 0; j < 4; j++)
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
                                _dungeonScene.AddEntity(entity);
                            }
                            else if (roomGen.RoomModels[i, j].RoomLayout[k, l] == "P")
                            {
                                // Ladder top
                                entity = new Entity(new Transform(new Vector2(y, x), new Vector2(1f, 1f), 0f), i + " " + k + " " + j + " " + l);
                                entity.AddComponent(Assets.GetSpriteSheet("TileMap.TilesetImages.DungeonTileSet.png").GetSprite(0, SpriteLayer.Background, false));
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
            gameWindow.RenderFrame += _ => gameWindow.SwapBuffers();
            gameWindow.Resize += Resize;
        }

        private void AddPortalEntities()
        {
            IEntity villageToDungeon = new Entity(new Transform(Constants.VILLAGE_SPAWN_POINT + new Vector2(5, 0), 1f, 0f), "villageToDungeonPortal");
            villageToDungeon.AddComponent(new Trigger(TriggerLayerType.PORTAL, 1f, 2f));
            villageToDungeon.AddComponent(new Portal("DungeonScene", Constants.DUNGEON_SPAWN_POINT));
            IEntity dungeonToVillage = new Entity(new Transform(new Vector2(25f, 1), 1f, 0f), "dungeonToVillagePortal");
            dungeonToVillage.AddComponent(new Trigger(TriggerLayerType.PORTAL, 1f, 2f));
            dungeonToVillage.AddComponent(new Portal("VillageScene", Constants.VILLAGE_SPAWN_POINT));

            _villageScene.AddEntity(villageToDungeon);
            _dungeonScene.AddEntity(dungeonToVillage);
        }

        private void Resize(ResizeEventArgs args)
        {
            Camera.Resize(args.Width, args.Height);
            GL.Viewport(0, 0, args.Width, args.Height);
        }

        private void CreatePlayerEntity()
        {
            Transform transform = new Transform(Constants.VILLAGE_SPAWN_POINT, 0.5f, 0f);
            _player = new Entity(transform, "Marin");
            _player.AddComponent(new Physics());
            _player.AddComponent(new Collider(new Vector2(0f, 0f), transform, 1f, 1f));
            _player.AddComponent(new Trigger(TriggerLayerType.FRIEND | TriggerLayerType.PORTAL, Vector2.Zero, 0.5f, 0.5f));
            _player.AddComponent(new DynamicCollider(_player.GetComponent<Collider>()));
            _player.AddComponent(new Player());
            _player.AddComponent(new Effect());
            _player.AddComponent(new Health(Constants.PLAYER_MAX_HEALTH));
            _player.AddComponent(new Sprite(Assets.GetTexture("Sprites.Player.png"), SpriteLayer.Foreground, true));

            // Setup player animations
            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_idle.png", 0.5f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_up.png", 0.08f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_down.png", 0.08f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_left.png", 0.08f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_right.png", 0.08f));
            _player.AddComponent(animController);
        }

        private void SetupMainMenuScene()
        {
            // Show background image
            Entity villageRenderEntity = new Entity(new Transform(new Vector2(0, 0), 0.3f, 0f), "VillageRender");
            villageRenderEntity.AddComponent(new Sprite(Assets.GetTexture("VillageRender.png"), SpriteLayer.Background, false));
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

        private void AddVillageSystems()
        {
            TileMapDictionary villageTileMap = new TileMapDictionary("Village.tmx");
            _villageScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            _villageScene.AddSystem(new CollisionSystem("Collision"));
            _villageScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _villageScene.AddSystem(new TriggerSystem("Trigger"));
            _villageScene.AddSystem(new PortalSystem("PortalSystem"));
            _villageScene.AddSystem(new CameraSystem("CameraSystem"));
            _villageScene.AddSystem(new SpriteRenderer("SpriteRenderer", true));
            _villageScene.AddSystem(new PlayerAnimationControllerSystem("AnimationControllerSystem"));
            _villageScene.AddSystem(new AnimationSystem("AnimationSystem"));
            _villageScene.AddSystem(new GUIInputSystem("GUIInputSystem"));
            _villageScene.SetTileMap(villageTileMap, false);
        }

        private void AddDungeonSystems()
        {
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
            _dungeonScene.AddSystem(new SpriteRenderer("SpriteRenderer", true));
            _dungeonScene.AddSystem(new AnimationSystem("AnimationSystem"));
            _dungeonScene.AddSystem(new HealthbarSystem("PlayerHealthbar", Constants.PLAYER_MAX_HEALTH));
            _dungeonScene.SetTileMap(tileMap, true);
        }

        private void AddGUIEntities()
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

        private void UpdateFrame(FrameEventArgs args)
        {
            _fps!.SetFps((float)args.Time);
            Time.SetTime((float)args.Time);
            Manager.GetInstance().Update((float)args.Time);
            MouseHandler.ClickedMouseButtons.Clear();
        }

        private void RenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Manager.GetInstance().Render();
        }
    }
}
