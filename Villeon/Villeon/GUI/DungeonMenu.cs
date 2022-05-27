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
            Sprite selectionSprite = Assets.GetSprite("GUI.Scroll_Selection.png", Render.SpriteLayer.ScreenGuiMiddleground, false);

            // Background
            Vector2 scrollMiddle = new Vector2(backgroundScrollSprite.Width / 2f, backgroundScrollSprite.Height / 2f);
            Entity backgroundImage = new Entity(new Transform(Vector2.Zero - (scrollMiddle * 0.5f), 0.5f, 0f), "BackgroundImage");
            backgroundImage.AddComponent(backgroundScrollSprite);
            _entities.Add(backgroundImage);

            // Horizontal Lines
            Entity horizontalLine1 = new Entity(new Transform(new Vector2(-6.3f, 2.6f), 0.5f, 0f), "Horizontal Line 1");
            horizontalLine1.AddComponent(horizontalLine1Sprite);
            Entity horizontalLine2 = new Entity(new Transform(new Vector2(-6.3f, 1), 0.5f, 0f), "Horizontal Line 2");
            horizontalLine2.AddComponent(horizontalLine2Sprite);
            Entity horizontalLine3 = new Entity(new Transform(new Vector2(-6.3f, -0.6f), 0.5f, 0f), "Horizontal Line 3");
            horizontalLine3.AddComponent(horizontalLine3Sprite);

            _entities.Add(horizontalLine1);
            _entities.Add(horizontalLine2);
            _entities.Add(horizontalLine3);

            // Menu Selection
            Entity selection = new Entity(new Transform(new Vector2(-6.4f, 3.1f), 0.5f, 0f), "Horizontal Line 1"); 
            selection.AddComponent(selectionSprite);
            _entities.Add(selection);

            // Text - Overview
            Text caveTitle = new Text("Crumbly Cave", new Vector2(-5.9f, 3.2f), 0.1f, 1.1f, 0.5f);
            Array.ForEach(caveTitle.GetEntities(), entity => _entities.Add(entity));
            Text darkLair = new Text("Darkend Lair", new Vector2(-5.9f, 1.7f), 0.1f, 1.1f, 0.5f);
            Array.ForEach(darkLair.GetEntities(), entity => _entities.Add(entity));
            Text swampyGrot = new Text("Swampy Grot", new Vector2(-5.9f, 0.1f), 0.1f, 1.1f, 0.5f);
            Array.ForEach(swampyGrot.GetEntities(), entity => _entities.Add(entity));
            Text hellishHole = new Text("Hellish Hole", new Vector2(-5.9f, -1.5f), 0.1f, 1.1f, 0.5f);
            Array.ForEach(hellishHole.GetEntities(), entity => _entities.Add(entity));

            // Text - Selection
            Text caveTitleSelection = new Text("Crumbly Cave", new Vector2(0.6f, 3.1f), 0.1f, 1.1f, 0.5f);
            Array.ForEach(caveTitleSelection.GetEntities(), entity => _entities.Add(entity));

            // Text - Description
            Text caveDescription = new Text("", new Vector2(0.6f, 2f), 0.1f, 0.5f, 0.25f);
            Array.ForEach(caveDescription.GetEntities(), entity => _entities.Add(entity));

            // Text - Explore
            Text explore = new Text("Go exploring", new Vector2(0.9f, -3.6f), 0.1f, 3f, 0.5f);
            Array.ForEach(explore.GetEntities(), entity => _entities.Add(entity));
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }
    }
}
