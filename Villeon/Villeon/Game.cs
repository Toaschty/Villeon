using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            SceneLoader.AddScene(_dungeonScene);
            SceneLoader.AddScene(_villageScene);
            SceneLoader.SetActiveScene("DungeonScene");

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
            floor.AddComponent(new Collider(new Vector2(-20, -2), 100f, 1f));
            _dungeonScene.AddEntity(floor);
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
            IEntity villageToDungeon = new Entity(new Transform(new Vector2(36, 32), 1f, 0f), "villageToDungeonPortal");
            villageToDungeon.AddComponent(TriggerBuilder.Build(TriggerID.DUNGEONENTRY));
            IEntity dungeonToVillage = new Entity(new Transform(new Vector2(25f, 1), 1f, 0f), "dungeonToVillagePortal");
            dungeonToVillage.AddComponent(TriggerBuilder.Build(TriggerID.VILLAGEENTRY));

            _dungeonScene.AddEntity(dungeonToVillage);
            _villageScene.AddEntity(villageToDungeon);
        }

        private void Resize(ResizeEventArgs args)
        {
            Camera.Resize(args.Width, args.Height);
            GL.Viewport(0, 0, args.Width, args.Height);
        }

        private void CreatePlayerEntity()
        {
            Transform transform = new Transform(Constants.DUNGEON_SPAWN_POINT, 0.3f, 0f);
            _player = new Entity(transform, "Marin");
            _player.AddComponent(new Physics());
            _player.AddComponent(new Collider(new Vector2(0f, 0f), transform, 1f, 1f));
            _player.AddComponent(new DynamicCollider(_player.GetComponent<Collider>()));
            _player.AddComponent(TriggerBuilder.Build(TriggerID.PLAYER));
            _player.AddComponent(new Player());
            _player.AddComponent(new Effect());
            _player.AddComponent(new Health(Constants.PLAYER_MAX_HEALTH));
            _player.AddComponent(new Sprite(Assets.GetTexture("Sprites.Player.png"), SpriteLayer.Foreground, true));

            // Setup player animations
            AnimationController animController = new AnimationController();
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_idle.png", 0.5f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_up.png", 0.1f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_down.png", 0.1f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_left.png", 0.1f));
            animController.AddAnimation(AnimationLoader.CreateAnimationFromFile("Animations.player_walk_right.png", 0.1f));
            _player.AddComponent(animController);
        }

        private void AddVillageSystems()
        {
            TileMapDictionary villageTileMap = new TileMapDictionary("VillageTileMap.tmx");
            _villageScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            _villageScene.AddSystem(new CollisionSystem("Collision"));
            _villageScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            _villageScene.AddSystem(new TriggerSystem("Trigger"));
            _villageScene.AddSystem(new CameraSystem("CameraSystem"));
            _villageScene.AddSystem(new HealthSystem("HealthSystem"));
            _villageScene.AddSystem(new SpriteRenderer("SpriteRenderer", true));
            _villageScene.AddSystem(new PlayerAnimationControllerSystem("AnimationControllerSystem"));
            _villageScene.AddSystem(new AnimationSystem("AnimationSystem"));
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
            GUI.Image dungeon_button = new GUI.Image("Dungeon_Button.png", new Vector2(-0.97f, -0.97f), new Vector2(0.04f, 0.04f));
            GUI.Image map_button = new GUI.Image("Map_Button.png", new Vector2(-0.77f, -0.97f), new Vector2(0.04f, 0.04f));
            GUI.Image equipment_button = new GUI.Image("Equipment_Button.png", new Vector2(-0.57f, -0.97f), new Vector2(0.04f, 0.04f));
            GUI.Image inventar_button = new GUI.Image("Inventar_Button.png", new Vector2(-0.37f, -0.97f), new Vector2(0.04f, 0.04f));

            _villageScene.AddEntities(dungeon_button.Entity, map_button.Entity, equipment_button.Entity, inventar_button.Entity);

            // Menu View - Village
        }

        private void UpdateFrame(FrameEventArgs args)
        {
            Manager.GetInstance().Update((float)args.Time);
            MouseHandler.ClickedMouseButtons.Clear();
        }

        private void RenderFrame(FrameEventArgs args)
        {
            _fps!.SetFps((float)args.Time);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Manager.GetInstance().Render();
        }
    }
}
