﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.GUI;
using Villeon.Helper;
using Villeon.Render;
using Villeon.Systems;
using Villeon.Systems.Update;
using Villeon.Utils;

namespace Villeon.ECS
{
    public static class Scenes
    {
        public static Scene MainMenuScene { get; } = new ("MainMenuScene");

        public static Scene LoadingScene { get; } = new ("LoadingScene");

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
            villageRenderEntity.AddComponent(new Sprite(Assets.GetTexture("Sprites.VillageRender.png"), SpriteLayer.Background, false));
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
            VillageScene.AddSystem(new SpriteRenderer("SpriteRenderer", true));
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
            DungeonScene.AddSystem(new SpriteRenderer("SpriteRenderer", true));
            DungeonScene.AddSystem(new AnimationSystem("AnimationSystem"));
            DungeonScene.AddSystem(new LadderSystem("LadderSystem"));
            DungeonScene.AddSystem(new MobDropSystem("MobdropSystem"));
            DungeonScene.AddSystem(new MobDropCollectionSystem("MobdropCollectionSystem"));
            DungeonScene.AddSystem(new HealthbarSystem("PlayerHealthbar", Constants.PLAYER_MAX_HEALTH));
            DungeonScene.SetTileMap(tileMap, true);
        }
    }
}
