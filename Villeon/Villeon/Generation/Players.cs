using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.Generation
{
    public class Players
    {
        public static IEntity CreateDungeonPlayer(Vector2 spawnPoint)
        {
            IEntity player;
            Transform transform = new Transform(spawnPoint, 0.5f, 0f);
            player = new Entity(transform, "DungeonMarin");
            player.AddComponent(new DynamicCollider(new Vector2(0f, 0f), transform, 1f, 1.5f));
            player.AddComponent(new Trigger(TriggerLayerType.FRIEND | TriggerLayerType.PORTAL | TriggerLayerType.LADDER | TriggerLayerType.MOBDROP, new Vector2(0f, 0f), 1f, 2f));
            player.AddComponent(new Sprite(Asset.GetTexture("Sprites.Empty.png"), SpriteLayer.Foreground, true));
            player.AddComponent(new Physics());
            player.AddComponent(new Effect());
            player.AddComponent(new Player());
            player.AddComponent(new Fokus());
            player.AddComponent(new Light(Color4.White, -12, 4f, new Vector2(0.5f, 1f)));
            player.AddComponent(new Health(Stats.GetInstance().GetMaxHealth()));

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

        public static IEntity CreateVillagePlayer(Vector2 spawnPoint)
        {
            IEntity player;
            Transform transform = new Transform(spawnPoint, 0.25f, 0f);
            player = new Entity(transform, "VillageMarin");
            player.AddComponent(new DynamicCollider(new Vector2(0f, 0f), transform, 0.5f, 0.5f));
            player.AddComponent(new Trigger(TriggerLayerType.FRIEND | TriggerLayerType.PORTAL, new Vector2(0f, 0f), 0.5f, 0.5f));
            player.AddComponent(new Sprite(Asset.GetTexture("Sprites.Empty.png"), SpriteLayer.Foreground, true));
            player.AddComponent(new Light(Color4.White));
            player.AddComponent(new Player());
            player.AddComponent(new Fokus());

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
    }
}
