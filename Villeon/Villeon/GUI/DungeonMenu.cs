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

            // Load Sprites
            Sprite backgroundScrollSprite = Assets.GetSprite("GUI.Scroll_Dungeonmenu.png", Render.SpriteLayer.ScreenGuiBackground, false);
            Sprite horizontalLine1Sprite = Assets.GetSprite("GUI.Scroll_Horizontal_Line_1.png", Render.SpriteLayer.ScreenGuiMiddleground, false);
            Sprite horizontalLine2Sprite = Assets.GetSprite("GUI.Scroll_Horizontal_Line_2.png", Render.SpriteLayer.ScreenGuiMiddleground, false);
            Sprite horizontalLine3Sprite = Assets.GetSprite("GUI.Scroll_Horizontal_Line_3.png", Render.SpriteLayer.ScreenGuiMiddleground, false);
            Sprite verticalLineSprite = Assets.GetSprite("GUI.Scroll_Vertical_Line_1.png", Render.SpriteLayer.ScreenGuiMiddleground, false);

            // Background
            Vector2 scrollMiddle = new Vector2(backgroundScrollSprite.Width / 2f, backgroundScrollSprite.Height / 2f);
            Entity backgroundImage = new Entity(new Transform(Vector2.Zero - (scrollMiddle * 0.5f), 0.5f, 0f), "BackgroundImage");
            backgroundImage.AddComponent(backgroundScrollSprite);
            _entities.Add(backgroundImage);

            // Horizontal Lines
            Entity hLine1 = new Entity(new Transform(new Vector2(-6f, 2), 0.5f, 0f), "Horizontal Line 1");
            hLine1.AddComponent(horizontalLine1Sprite);
            Entity hLine2 = new Entity(new Transform(new Vector2(-6f, 0), 0.5f, 0f), "Horizontal Line 2");
            hLine2.AddComponent(horizontalLine2Sprite);
            Entity hLine3 = new Entity(new Transform(new Vector2(-6f, -2), 0.5f, 0f), "Horizontal Line 3");
            hLine3.AddComponent(horizontalLine3Sprite);
            Entity hLine4 = new Entity(new Transform(new Vector2(-6f, -4), 0.5f, 0f), "Horizontal Line 4");
            hLine4.AddComponent(horizontalLine2Sprite);

            _entities.Add(hLine1);
            _entities.Add(hLine2);
            _entities.Add(hLine3);
            _entities.Add(hLine4);

            // Text - Overview
            Text caveTitle = new Text("Crumbly Cave", new Vector2(-0.48f, 0.61f), 0.9f, 1.1f, 0.04f);
            Array.ForEach(caveTitle.GetEntities(), entity => _entities.Add(entity));
            Text darkLair = new Text("Darkend Lair", new Vector2(-0.48f, 0.38f), 0.9f, 1.1f, 0.04f);
            Array.ForEach(darkLair.GetEntities(), entity => _entities.Add(entity));
            Text swampyGrot = new Text("Swampy Grot", new Vector2(-0.48f, 0.15f), 0.9f, 1.1f, 0.04f);
            Array.ForEach(swampyGrot.GetEntities(), entity => _entities.Add(entity));
            Text hellishHole = new Text("Hellish Hole", new Vector2(-0.48f, -0.08f), 0.8f, 1.1f, 0.04f);
            Array.ForEach(hellishHole.GetEntities(), entity => _entities.Add(entity));

            // Text - Selection
            Text caveTitleSelection = new Text("Crumbly Cave", new Vector2(0.05f, 0.61f), 0.9f, 1.1f, 0.04f);
            Array.ForEach(caveTitleSelection.GetEntities(), entity => _entities.Add(entity));

            // Text - Description
            String desciption = "This cave is located in\n" +
                                "the nearby woods. The\n" +
                                "entrance is often buried\n" +
                                "under a pile of gravel.\n" +
                                "The villagers themselves\n" +
                                "even dont know where this\n" +
                                "is coming from. Due to the\n" +
                                "lack of name ideas - It is\n" +
                                "now called 'Crumbly Cave'.";
            Text caveDescription = new Text(desciption, new Vector2(0.05f, 0.4f), 0.9f, 3f, 0.02f, -0.005f);
            Array.ForEach(caveDescription.GetEntities(), entity => _entities.Add(entity));

            // Text - Explore
            Text explore = new Text("Go exploring", new Vector2(0.1f, -0.35f), 0.9f, 3f, 0.03f);
            Array.ForEach(explore.GetEntities(), entity => _entities.Add(entity));
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }
    }
}
