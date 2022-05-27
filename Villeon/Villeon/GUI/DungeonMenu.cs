using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.GUI
{
    public class DungeonMenu : IGUIMenu
    {
        private List<Entity> _entities;

        private Entity _menuSelection;

        private Vector2 _overviewSelectionStartPosition = new Vector2(-6.4f, 3.1f);
        private Vector2 _overviewLineStartPostition = new Vector2(-6.3f, 2.6f);
        private Vector2 _overviewStartPosition = new Vector2(-5.9f, 3.2f);
        private Vector2 _overviewSpacing = new Vector2(0, 1.5f);

        private float _letterScaleBig = 0.5f;
        private float _letterScaleSmall = 0.25f;

        private int _currentSelection = 0;

        public DungeonMenu()
        {
            // Create Menu layout
            _entities = new List<Entity>();

            // Load cave data
            dynamic cavesJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("GUI.DungeonMenu.json")) !;

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

            // Menu Selection
            _menuSelection = new Entity(new Transform(_overviewSelectionStartPosition - (_currentSelection * _overviewSpacing), 0.5f, 0f), "Selection");
            _menuSelection.AddComponent(selectionSprite);
            _entities.Add(_menuSelection);

            // Fill in dynamic data
            for (int i = 0; i < cavesJson.caves.Count; i++)
            {
                // Text
                Text overViewText = new Text(cavesJson.caves[i].name.ToString(), _overviewStartPosition - (i * _overviewSpacing), 0.1f, 0.5f, _letterScaleBig);
                Array.ForEach(overViewText.GetEntities(), entity => _entities.Add(entity));

                // Line
                if (i < cavesJson.caves.Count - 1)
                {
                    Entity horizontalLine = new Entity(new Transform(_overviewLineStartPostition - (i * _overviewSpacing), 0.5f, 0f), "Horizontal Line");
                    horizontalLine.AddComponent(horizontalLine1Sprite);
                    _entities.Add(horizontalLine);
                }
            }

            /*
            // Text - Overview
            Text caveTitle = new Text(cavesJson.caves[0].name.ToString(), _overviewStartPosition, 0.1f, 0.5f, _letterScaleBig);
            Array.ForEach(caveTitle.GetEntities(), entity => _entities.Add(entity));
            Text darkLair = new Text("Darkend Lair", new Vector2(-5.9f, 1.7f), 0.1f, 0.5f, _letterScaleBig);
            Array.ForEach(darkLair.GetEntities(), entity => _entities.Add(entity));
            Text swampyGrot = new Text("Swampy Grot", new Vector2(-5.9f, 0.1f), 0.1f, 0.5f, _letterScaleBig);
            Array.ForEach(swampyGrot.GetEntities(), entity => _entities.Add(entity));
            Text hellishHole = new Text("Hellish Hole", new Vector2(-5.9f, -1.5f), 0.1f, 0.5f, _letterScaleBig);
            Array.ForEach(hellishHole.GetEntities(), entity => _entities.Add(entity));

            // Text - Selection
            Text caveTitleSelection = new Text(cavesJson.caves[0].name.ToString(), new Vector2(0.6f, 3.2f), 0.1f, 0.5f, _letterScaleBig);
            Array.ForEach(caveTitleSelection.GetEntities(), entity => _entities.Add(entity));

            // Text - Description
            Text caveDescription = new Text(cavesJson.caves[0].description.ToString(), new Vector2(0.6f, 2f), 0.1f, 0.5f, _letterScaleSmall);
            Array.ForEach(caveDescription.GetEntities(), entity => _entities.Add(entity));
            */

            // Text - Explore
            Text explore = new Text("Go exploring", new Vector2(0.9f, -3.6f), 0.1f, 3f, 0.5f);
            Array.ForEach(explore.GetEntities(), entity => _entities.Add(entity));
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
