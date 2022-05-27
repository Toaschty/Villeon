using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.GUI
{
    public class EquipmentMenu : IGUIMenu
    {
        private List<Entity> _entities;

        public EquipmentMenu()
        {
            // Create Menu layout
            _entities = new List<Entity>();

            // Background
            float scrollScale = 0.5f;
            Sprite backgroundSprite = Assets.GetSprite("GUI.Scroll.png", Render.SpriteLayer.ScreenGuiBackground, false);
            Vector2 middle = new Vector2(backgroundSprite.Width / 2f, backgroundSprite.Height / 2f);
            middle *= scrollScale;
            Entity backgroundImage = new Entity(new Transform(Vector2.Zero - middle, scrollScale, 0f), "BackgroundImage");
            backgroundImage.AddComponent(backgroundSprite);
            _entities.Add(backgroundImage);

            // Image
            Entity image = new Entity(new Transform(new Vector2(-3f, 0f), 0.5f, 0f), "Image");
            Sprite player = Assets.GetSprite("Animations.player_walk_left.png", Render.SpriteLayer.ScreenGuiMiddleground, false);
            image.AddComponent(player);
            _entities.Add(image);
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }

        public void OnKeyReleased(Keys key)
        {
            throw new NotImplementedException();
        }
    }
}
