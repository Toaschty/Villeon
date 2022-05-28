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
        private Vector2 _onExplorePosition = new Vector2(0.4f, -3.7f);

        private float _letterScaleBig = 0.35f;
        private float _letterScaleSmall = 0.15f;

        private int _currentSelection = 0;
        private bool _onExplore = false;
        private int _elementCount = 0;

        private Text _title;
        private Text _description;

        private dynamic _cavesJson;

        public DungeonMenu()
        {
            // Create Menu layout
            _entities = new List<Entity>();

            // Load cave data
            _cavesJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("GUI.DungeonMenu.json")) !;
            _elementCount = _cavesJson.caves.Count;

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
            for (int i = 0; i < _elementCount; i++)
            {
                // Text
                Text overViewText = new Text(_cavesJson.caves[i].name.ToString(), _overviewStartPosition - (i * _overviewSpacing), 0.1f, 0.5f, _letterScaleBig);
                Array.ForEach(overViewText.GetEntities(), entity => _entities.Add(entity));

                // Line
                if (i < _elementCount - 1)
                {
                    Entity horizontalLine = new Entity(new Transform(_overviewLineStartPostition - (i * _overviewSpacing), 0.5f, 0f), "Horizontal Line");
                    horizontalLine.AddComponent(horizontalLine1Sprite);
                    _entities.Add(horizontalLine);
                }
            }

            // Text - Explore
            Text explore = new Text("Go explore", new Vector2(0.3f, -3.8f), 0.1f, 3f, 0.5f);
            Array.ForEach(explore.GetEntities(), entity => _entities.Add(entity));

            // Load in first text
            LoadText();
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }

        public bool OnKeyReleased(Keys key)
        {
            // Overview movement
            if (key == Keys.W && !_onExplore)
            {
                // Adjust selection
                _currentSelection--;
                if (_currentSelection < 0)
                    _currentSelection = _elementCount - 1;

                // Move position
                UpdateSelectionPosition();

                // Update text
                UpdateText();
            }

            if (key == Keys.S && !_onExplore)
            {
                // Adjust selection
                _currentSelection++;
                if (_currentSelection >= _elementCount)
                    _currentSelection = 0;

                // Move position
                UpdateSelectionPosition();

                // Update text
                UpdateText();
            }

            if (key == Keys.D)
            {
                _onExplore = true;

                // Move position
                _menuSelection.GetComponent<Transform>().Position = _onExplorePosition;
            }

            if (key == Keys.A)
            {
                _onExplore = false;

                // Move position
                UpdateSelectionPosition();
            }

            if (key == Keys.Space && _onExplore)
            {
                Manager.GetInstance().RemoveEntities(GetEntities());
                SceneLoader.SetActiveScene("DungeonScene");
                return false;
            }

            return true;
        }

        // Unload existing text and load in new code
        private void UpdateText()
        {
            UnloadText();
            LoadText();
        }

        private void UnloadText()
        {
            // Remove all title letters from the scene and the local list of entities
            Array.ForEach(_title.GetEntities(), entity =>
            {
                _entities.Remove(entity);
                Manager.GetInstance().RemoveEntity(entity);
            });

            // Remove all description letters from the scene and the local list of entities
            Array.ForEach(_description.GetEntities(), entity =>
            {
                _entities.Remove(entity);
                Manager.GetInstance().RemoveEntity(entity);
            });
        }

        private void LoadText()
        {
            // Title
            _title = new Text(_cavesJson.caves[_currentSelection].name.ToString(), new Vector2(0.6f, 3.2f), 0.05f, 0.5f, _letterScaleBig);
            Array.ForEach(_title.GetEntities(), entity => _entities.Add(entity));

            // Description
            _description = new Text(_cavesJson.caves[_currentSelection].description.ToString(), new Vector2(0.6f, 2.1f), 0.00f, 0.5f, _letterScaleSmall);
            Array.ForEach(_description.GetEntities(), entity => _entities.Add(entity));
        }

        private void UpdateSelectionPosition()
        {
            _menuSelection.GetComponent<Transform>().Position = _overviewSelectionStartPosition - (_currentSelection * _overviewSpacing);
        }
    }
}
