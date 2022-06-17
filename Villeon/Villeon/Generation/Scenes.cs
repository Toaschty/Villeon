using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;
using Villeon.Helper;
using Villeon.Systems.RenderSystems;
using Villeon.Systems.TriggerSystems;
using Villeon.Systems.Update;

namespace Villeon.Generation
{
    public static class Scenes
    {
        public static Scene MainMenuScene { get; } = new Scene("MainMenuScene");

        public static Scene LoadingScene { get; } = new Scene("LoadingScene");

        public static Scene DungeonScene { get; } = new Scene("DungeonScene");

        public static Scene VillageScene { get; } = new Scene("VillageScene");

        public static Scene SmithScene { get; } = new Scene("SmithScene");

        public static Scene ShopScene { get; } = new Scene("ShopScene");

        public static void SetupSmithScene()
        {
            SceneLoader.AddScene(SmithScene);

            TileMapDictionary smithTileMap = new TileMapDictionary("VillageSmith.tmx");
            SmithScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            SmithScene.AddSystem(new CollisionSystem("Collision"));
            SmithScene.AddSystem(new TriggerSystem("Trigger"));
            SmithScene.AddSystem(new PortalSystem("PortalSystem"));
            SmithScene.AddSystem(new CameraSystem("CameraSystem"));
            SmithScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));
            SmithScene.AddSystem(new PlayerAnimationControllerSystem("AnimationControllerSystem"));
            SmithScene.AddSystem(new AnimationSystem("AnimationSystem"));
            SmithScene.AddSystem(new GUIInputSystem("GUIInputSystem"));
            SmithScene.SetTileMap(smithTileMap, false);
        }

        public static void SetupMainMenuScene()
        {
            // Show background image
            Entity villageRenderEntity = new Entity(new Transform(new Vector2(0, 0), 0.3f, 0f), "VillageRender");
            villageRenderEntity.AddComponent(new Sprite(Asset.GetTexture("Sprites.VillageRender.png"), SpriteLayer.Background, false));
            MainMenuScene.AddEntity(villageRenderEntity);

            // Enable camera movement
            Entity cameraMovementEntity = new Entity(new Transform(Vector2.Zero, 1f, 0f), "CameraMovement");
            cameraMovementEntity.AddComponent(new AutoCameraMovement(new Vector2(115f, 70f), 50f, 0.02f));
            MainMenuScene.AddEntity(cameraMovementEntity);

            // Main menu GUI Setup
            new MainMenuSetup();

            Entity selectorEntity = new Entity(new Transform(Vector2.Zero, 1f, 0f), "Selector");
            selectorEntity.AddComponent(new MainMenu());
            MainMenuScene.AddEntity(selectorEntity);

            StateManager.InMenu = true;

            // Add required systems
            MainMenuScene.AddSystem(new CameraSystem("CameraSystem"));
            MainMenuScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));
            MainMenuScene.AddSystem(new AutoCameraMovementSystem("MovingCamera"));
            MainMenuScene.AddSystem(new MainMenuInputSystem("MainMenuInput"));
        }

        public static void SetupShopScene()
        {
            SceneLoader.AddScene(ShopScene);

            TileMapDictionary shopTileMap = new TileMapDictionary("VillageShop.tmx");
            ShopScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            ShopScene.AddSystem(new CollisionSystem("Collision"));
            ShopScene.AddSystem(new TriggerSystem("Trigger"));
            ShopScene.AddSystem(new PortalSystem("PortalSystem"));
            ShopScene.AddSystem(new CameraSystem("CameraSystem"));
            ShopScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));
            ShopScene.AddSystem(new PlayerAnimationControllerSystem("AnimationControllerSystem"));
            ShopScene.AddSystem(new AnimationSystem("AnimationSystem"));
            ShopScene.AddSystem(new GUIInputSystem("GUIInputSystem"));
            ShopScene.SetTileMap(shopTileMap, false);
        }

        public static void SetupVillageScene()
        {
            SceneLoader.AddScene(VillageScene);

            TileMapDictionary villageTileMap = new TileMapDictionary("Village.tmx");
            VillageScene.AddSystem(new PlayerTopDownMovementSystem("TopDownMovement"));
            VillageScene.AddSystem(new CollisionSystem("Collision"));
            VillageScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            VillageScene.AddSystem(new TriggerSystem("Trigger"));
            VillageScene.AddSystem(new PortalSystem("PortalSystem"));
            VillageScene.AddSystem(new CameraSystem("CameraSystem"));
            VillageScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));
            VillageScene.AddSystem(new InteractionSystem("InteractionSystem"));
            VillageScene.AddSystem(new PlayerAnimationControllerSystem("AnimationControllerSystem"));
            VillageScene.AddSystem(new AnimationSystem("AnimationSystem"));
            VillageScene.AddSystem(new GUIInputSystem("GUIInputSystem"));
            VillageScene.SetTileMap(villageTileMap, false);
        }

        public static void SetupDungeonScene()
        {
            SceneLoader.AddScene(DungeonScene);

            TileMapDictionary tileMap = new TileMapDictionary("DungeonTileMap.tmx");
            DungeonScene.AddSystem(new EffectSystem("Effects"));
            DungeonScene.AddSystem(new PlayerMovementSystem("Move"));
            DungeonScene.AddSystem(new MouseClickSystem("MouseClickSystem"));
            DungeonScene.AddSystem(new SimpleAISystem("SimpleAISystem"));
            DungeonScene.AddSystem(new PhysicsSystem("Physics"));
            DungeonScene.AddSystem(new TriggerSystem("Trigger"));
            DungeonScene.AddSystem(new PortalSystem("PortalSystem"));
            DungeonScene.AddSystem(new DamageSystem("DamageSystem"));
            DungeonScene.AddSystem(new CollisionSystem("Collision"));
            DungeonScene.AddSystem(new PlayerDeathSystem("Health"));
            DungeonScene.AddSystem(new CameraSystem("CameraSystem"));
            DungeonScene.AddSystem(new SpriteRenderer("SpriteRenderer", false));
            DungeonScene.AddSystem(new EnemyHealthbarSystem("EnemyHealthbarSystem"));
            DungeonScene.AddSystem(new AnimationSystem("AnimationSystem"));
            DungeonScene.AddSystem(new LadderSystem("LadderSystem"));
            DungeonScene.AddSystem(new MobDropSystem("MobdropSystem"));
            DungeonScene.AddSystem(new MobDropCollectionSystem("MobdropCollectionSystem"));
            DungeonScene.AddSystem(new GUIInputSystem("GUIInputSystem"));
            DungeonScene.AddSystem(new PlayerHealthbarSystem("PlayerHealthbar", Constants.PLAYER_MAX_HEALTH));
            DungeonScene.AddSystem(new ItemUseSystem("ItemUseSystem"));
            DungeonScene.SetTileMap(tileMap, true);
        }

        public static void SetupPortalEntities()
        {
            IEntity villageToDungeon = new Entity(new Transform(Constants.VILLAGE_SPAWN_POINT + new Vector2(5, 0), 1f, 0f), "villageToDungeonPortal");
            villageToDungeon.AddComponent(new Trigger(TriggerLayerType.PORTAL, 1f, 2f));
            villageToDungeon.AddComponent(new Portal("DungeonScene", Constants.VILLAGE_SPAWN_POINT));
            VillageScene.AddEntity(villageToDungeon);

            IEntity dungeonToVillage = new Entity(new Transform(new Vector2(1f, 3f), 1f, 0f), "dungeonToVillagePortal");
            dungeonToVillage.AddComponent(new Trigger(TriggerLayerType.PORTAL, 1f, 4f));
            dungeonToVillage.AddComponent(new Portal("VillageScene", Constants.DUNGEON_SPAWN_POINT));
            DungeonScene.AddEntity(dungeonToVillage);

            IEntity villageToSmith = new Entity(new Transform(Constants.TO_SMITH_PORTAL_POINT, 1f, 0f), "VillageToSmithPortal");
            villageToSmith.AddComponent(new Trigger(TriggerLayerType.PORTAL, 2f, 1f));
            villageToSmith.AddComponent(new Portal("SmithScene", Constants.TO_SMITH_PORTAL_POINT + new Vector2(1f, -1f)));
            VillageScene.AddEntity(villageToSmith);

            IEntity smithToVillage = new Entity(new Transform(Constants.FROM_SMITH_PORTAL_POINT, 1f, 0f), "SmithToVillagePortal");
            smithToVillage.AddComponent(new Trigger(TriggerLayerType.PORTAL, 2f, 1.5f));
            smithToVillage.AddComponent(new Portal("VillageScene", Constants.FROM_SMITH_PORTAL_POINT + new Vector2(1f, 2f)));
            SmithScene.AddEntity(smithToVillage);

            IEntity villageToShop = new Entity(new Transform(Constants.TO_SHOP_PORTAL_POINT, 1f, 0f), "VillageToShopPortal");
            villageToShop.AddComponent(new Trigger(TriggerLayerType.PORTAL, 2f, 1f));
            villageToShop.AddComponent(new Portal("ShopScene", Constants.TO_SHOP_PORTAL_POINT + new Vector2(1f, -1f)));
            VillageScene.AddEntity(villageToShop);

            IEntity shopToVillage = new Entity(new Transform(Constants.FROM_SHOP_PORTAL_POINT, 1f, 0f), "ShopToVillagePortal");
            shopToVillage.AddComponent(new Trigger(TriggerLayerType.PORTAL, 2f, 1.5f));
            shopToVillage.AddComponent(new Portal("VillageScene", Constants.FROM_SHOP_PORTAL_POINT + new Vector2(1f, 2f)));
            ShopScene.AddEntity(shopToVillage);
        }
    }
}
