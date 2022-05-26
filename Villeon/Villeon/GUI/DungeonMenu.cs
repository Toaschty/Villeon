using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.GUI
{
    public class DungeonMenu : IGUIMenu
    {
        private List<Entity> _entities;

        public DungeonMenu()
        {
            // Create Menu layout
            _entities = new List<Entity>();

            // Background
            Entity backgroundImage = new Entity(new Transform(new Vector2(-0.755f, -0.6f), 0.04f, 0f), "BackgroundImage");
            Sprite backgroundSprite = Assets.GetSprite("GUI.Scroll.png", Render.SpriteLayer.ScreenGuiBackground, false);
            backgroundImage.AddComponent(backgroundSprite);
            _entities.Add(backgroundImage);

            // Image
            Entity image = new Entity(new Transform(new Vector2(-0.755f, -0.6f), 0.04f, 0f), "Image");
            Sprite player = Assets.GetSprite("Animations.player_idle.png", Render.SpriteLayer.ScreenGuiMiddleground, false);
            image.AddComponent(player);
            _entities.Add(image);
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }
    }
}
