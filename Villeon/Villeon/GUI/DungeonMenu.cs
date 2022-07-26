using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.GUI
{
    public class DungeonMenu : IGUIMenu
    {
        private static int _currentSelection = 0;

        private List<IEntity> _entities;

        private Entity _menuSelection;

        // Positions and offsets for gui elements
        private Vector2 _overviewSelectionStartPosition = new Vector2(-6.4f, 3.1f);
        private Vector2 _overviewLineStartPostition = new Vector2(-6.3f, 2.6f);
        private Vector2 _overviewStartPosition = new Vector2(-5.9f, 3.2f);
        private Vector2 _overviewSpacing = new Vector2(0, 1.5f);
        private Vector2 _onExplorePosition = new Vector2(0.4f, -3.7f);

        // Text size
        private float _letterScaleBig = 0.35f;
        private float _letterScaleSmall = 0.2f;

        // Selection variables
        private bool _onExplore = false;
        private int _elementCount = 0;

        private Text? _title;
        private Text? _description;
        private Text? _unlocks;

        // Holds Json file of cave data
        private dynamic _cavesJson;

        public DungeonMenu()
        {
            // Create Menu layout
            _entities = new List<IEntity>();

            // Load cave data
            _cavesJson = JsonConvert.DeserializeObject(ResourceLoader.LoadContentAsText("Jsons.DungeonMenu.json")) !;
            _elementCount = _cavesJson.caves.Count;

            // Load Sprites
            Sprite backgroundScrollSprite = Asset.GetSprite("GUI.Scrolls.Scroll_Dungeonmenu.png", SpriteLayer.ScreenGuiBackground, false);
            Sprite horizontalLine1Sprite = Asset.GetSprite("GUI.Scrolls.Scroll_Horizontal_Line_1.png", SpriteLayer.ScreenGuiMiddleground, false);
            Sprite horizontalLine2Sprite = Asset.GetSprite("GUI.Scrolls.Scroll_Horizontal_Line_2.png", SpriteLayer.ScreenGuiMiddleground, false);
            Sprite horizontalLine3Sprite = Asset.GetSprite("GUI.Scrolls.Scroll_Horizontal_Line_3.png", SpriteLayer.ScreenGuiMiddleground, false);
            Sprite verticalLineSprite = Asset.GetSprite("GUI.Scrolls.Scroll_Vertical_Line_1.png", SpriteLayer.ScreenGuiMiddleground, false);
            Sprite selectionSprite = Asset.GetSprite("GUI.Scrolls.Scroll_Selection.png", SpriteLayer.ScreenGuiMiddleground, false);

            // Background
            Vector2 scrollMiddle = new Vector2(backgroundScrollSprite.Width / 2f, backgroundScrollSprite.Height / 2f);
            IEntity backgroundImage = new Entity(new Transform(Vector2.Zero - (scrollMiddle * 0.5f), 0.5f, 0f), "BackgroundImage");
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
                Text overViewText = new Text(_cavesJson.caves[i].name.ToString(), _overviewStartPosition - (i * _overviewSpacing), "Alagard", 0f, 0.5f, _letterScaleBig);
                Array.ForEach(overViewText.GetEntities(), entity => _entities.Add(entity));

                // Line
                if (i < _elementCount - 1)
                {
                    Entity horizontalLine = new Entity(new Transform(_overviewLineStartPostition - (i * _overviewSpacing), 0.5f, 0f), "Horizontal Line");
                    horizontalLine.AddComponent(horizontalLine1Sprite);
                    _entities.Add(horizontalLine);
                }
            }

            // Text - Unlock
            Text unlock = new Text("/ 3", new Vector2(3.8f, -2.15f), "Alagard", 0f, 0.5f, _letterScaleBig);
            Array.ForEach(unlock.GetEntities(), entity => _entities.Add(entity));

            // Text - Explore
            Text explore = new Text("Go explore", _onExplorePosition + new Vector2(1.2f, 0), "Alagard", 0f, 3f, _letterScaleBig);
            Array.ForEach(explore.GetEntities(), entity => _entities.Add(entity));

            // Load in first text
            LoadText();
        }

        public static int Selection => _currentSelection;

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
            Array.ForEach(_title!.GetEntities(), entity =>
            {
                _entities.Remove(entity);
                Manager.GetInstance().RemoveEntity(entity);
            });

            // Remove all description letters from the scene and the local list of entities
            Array.ForEach(_description!.GetEntities(), entity =>
            {
                _entities.Remove(entity);
                Manager.GetInstance().RemoveEntity(entity);
            });

            // Remove all unlock letters from the scene and the local list of entities
            Array.ForEach(_unlocks!.GetEntities(), entity =>
            {
                _entities.Remove(entity);
                Manager.GetInstance().RemoveEntity(entity);
            });
        }

        private void LoadText()
        {
            // Title
            string title = _cavesJson.caves[_currentSelection].name.ToString();
            _title = new Text(title, new Vector2(0.6f, 3.2f), "Alagard", 0f, 0.5f, _letterScaleBig);
            Array.ForEach(_title.GetEntities(), entity => _entities.Add(entity));

            // Description
            string description = _cavesJson.caves[_currentSelection].description.ToString();
            _description = new Text(description, new Vector2(0.4f, 2.1f), "Alagard_Thin", 0f, 0.5f, _letterScaleSmall);
            Array.ForEach(_description.GetEntities(), entity => _entities.Add(entity));

            // Unlocks
            string unlocks = Stats.GetInstance().GetUnlockProgress(_currentSelection) + string.Empty;
            _unlocks = new Text(unlocks, new Vector2(3.2f, -2.15f), "Alagard", 0f, 0.5f, _letterScaleBig);
            Array.ForEach(_unlocks.GetEntities(), entity => _entities.Add(entity));
        }

        private void UpdateSelectionPosition()
        {
            _menuSelection.GetComponent<Transform>().Position = _overviewSelectionStartPosition - (_currentSelection * _overviewSpacing);
        }
    }
}
