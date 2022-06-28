using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.GUI
{
    public class HelpMenu : IGUIMenu
    {
        private List<IEntity> _entities;

        public HelpMenu()
        {
            _entities = new List<IEntity>();

            // Load Sprites
            Sprite helpScrollSprite = Asset.GetSprite("GUI.Help_Scroll.png", SpriteLayer.ScreenGuiBackground, false);

            // Background
            Vector2 scrollMiddle = new Vector2(helpScrollSprite.Width / 2f, helpScrollSprite.Height / 2f);
            Entity helpImage = new Entity(new Transform(Vector2.Zero - (scrollMiddle * 0.5f), 0.5f, 0f), "HelpImage");
            helpImage.AddComponent(helpScrollSprite);
            _entities.Add(helpImage);
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }

        public bool OnKeyReleased(Keys key)
        {
            return true;
        }
    }
}
